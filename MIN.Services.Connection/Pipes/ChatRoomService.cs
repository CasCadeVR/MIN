using System.Collections.Concurrent;
using System.Diagnostics;
using MIN.Services.Connection.Contracts.Interfaces.Discovering;
using MIN.Services.Connection.Contracts.Interfaces.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models.Exceptions;
using MIN.Services.Connection.Pipes.Discovering;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Events;

namespace MIN.Services.Connection.Pipes
{
    /// <summary>
    /// Сервис по работе с подключениями
    /// </summary>
    public class ChatRoomService : IChatRoomService, IAsyncDisposable
    {
        private readonly IPipeRoomServer server;
        private readonly IPipeParticipantClient client;
        private readonly IPipeMessageSerializer serializer;

        // Текущее состояние (только одно активное подключение)
        private Room? currentRoom;
        private Participant? selfParticipant;
        private bool isDisposed;
        private IDiscoveryServer discoveryServer;
        private IDiscoveryClient discoveryClient;

        private bool IsHost => selfParticipant?.PCName == currentRoom?.HostParticipant.PCName;

        // События для подписки UI
        public event EventHandler<ParticipantJoinedEventArgs>? ParticipantJoined;
        public event EventHandler<ParticipantLeftEventArgs>? ParticipantLeft;
        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
        public event EventHandler<RoomStateChangedEventArgs>? RoomStateChanged;
        public event EventHandler<ConnectionLostEventArgs>? ConnectionLost;

        public ChatRoomService(
            IPipeRoomServer server, 
            IPipeParticipantClient client,
            IPipeMessageSerializer serializer
        ) {
            this.server = server;
            this.client = client;
            this.serializer = serializer;

            this.client.MessageReceived += (s, e) => OnTransportMessageReceived(e);
            this.client.RoomInfoReceived += (s, e) => OnRoomInfoReceived(e);
            this.client.ParticipantJoined += (s, e) => OnTransportParticipantJoined(e);
            this.client.ParticipantLeft += (s, e) => OnTransportParticipantLeft(e);
            this.client.Disconnected += (s, e) => OnTransportDisconnected();
        }

        async Task<IEnumerable<Room>> IChatRoomService.DiscoverAvailableRoomsAsync(IEnumerable<string> targetPCNames, int timeoutMs = 1000, CancellationToken cancellationToken = default)
        {
            var discoveredRooms = new ConcurrentBag<Room>();
            var tasks = targetPCNames.Select(async pcName =>
            {
                try
                {
                    discoveryClient = new DiscoveryClient(serializer);
                    Debug.WriteLine($"Checking computer with name: {pcName}");
                    var room = await discoveryClient.DiscoverRoomAsync(pcName, TimeSpan.FromMilliseconds(timeoutMs));
                    discoveredRooms.Add(room!);
                }
                catch (RoomDiscoveryException ex)
                {
                    Debug.WriteLine($"Unable to discover room in {pcName}: {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);
            return discoveredRooms.ToList();
        }

        async Task<Room> IChatRoomService.CreateRoomAsync(string roomName, int maxParticipants, Participant host, CancellationToken cancellationToken = default)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ChatRoomService));

            await DisconnectAsync(cancellationToken);

            currentRoom = new Room(roomName, maxParticipants) { HostParticipant = host };
            selfParticipant = host;

            await server.StartAsync(currentRoom, cancellationToken);

            discoveryServer = new DiscoveryServer(host.PCName, server.Room, serializer);
            await discoveryServer.StartAsync(cancellationToken);

            OnRoomStateChanged(new RoomStateChangedEventArgs(currentRoom, RoomState.Created));

            return currentRoom;
        }

        async Task IChatRoomService.JoinRoomAsync(Room room, Participant participant, int timeoutMs = 1000, CancellationToken cancellationToken = default)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(ChatRoomService));
            }

            //await DisconnectAsync(cancellationToken);

            selfParticipant = participant;

            await client.ConnectAsync(room, participant, timeoutMs, cancellationToken);
            OnRoomStateChanged(new RoomStateChangedEventArgs(room, RoomState.Joined));
        }

        async Task IChatRoomService.SendMessageAsync(string content, MessageType type, CancellationToken cancellationToken = default)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(ChatRoomService));
            }

            if (currentRoom == null || selfParticipant == null)
                throw new InvalidOperationException("Not connected to any room");

            var message = new ChatMessage
            {
                SenderName = selfParticipant.Name,
                SenderPCName = selfParticipant.PCName,
                Content = content,
                MessageType = type,
                TimestampUtc = DateTime.UtcNow
            };

            await client.SendMessageAsync(message, cancellationToken);
        }

        /// <summary>
        /// Отключиться от комнаты
        /// </summary>
        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (isDisposed)
                return;

            try
            {
                if (IsHost && server.IsRunning)
                {
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync(cancellationToken);
                    }
                    await server.StopAsync();
                    await discoveryServer.StopAsync();
                }
                else if (!IsHost && client.IsConnected)
                {
                    await client.DisconnectAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during disconnect: {ex.Message}");
            }
            finally
            {
                currentRoom = null;
                selfParticipant = null;

                OnRoomStateChanged(new RoomStateChangedEventArgs(null, RoomState.Disconnected));
            }
        }

        /// <summary>
        /// Получена информация о комнате от сервера (при подключении клиента)
        /// </summary>
        private void OnRoomInfoReceived(RoomInfoMessage roomInfo)
        {
            currentRoom = new Room(roomInfo.RoomName, roomInfo.MaxParticipants)
            {
                Id = roomInfo.RoomId,
                HostParticipant = new Participant() {
                    Id = roomInfo.HostId,
                    Name = roomInfo.HostName,
                    PCName = roomInfo.HostPCName,
                }
            };

            foreach (var participant in roomInfo.CurrentParticipants)
            {
                currentRoom.CurrentParticipants.Add(participant);
            }

            OnRoomStateChanged(new RoomStateChangedEventArgs(currentRoom, RoomState.Joined));
        }

        /// <summary>
        /// Получено обычное сообщение от транспорта
        /// </summary>
        private void OnTransportMessageReceived(ChatMessage message)
        {
            currentRoom?.AddMessage(message);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        /// <summary>
        /// Участник присоединился к комнате
        /// </summary>
        private void OnTransportParticipantJoined(Participant participant)
        {
            currentRoom?.AddParticipant(participant);
            ParticipantJoined?.Invoke(this, new ParticipantJoinedEventArgs(participant));
        }

        /// <summary>
        /// Участник покинул комнату
        /// </summary>
        private void OnTransportParticipantLeft(Participant participant)
        {
            if (currentRoom?.RemoveParticipantById(participant.Id) == false)
            {
                throw new ArgumentNullException($"Не нашёлся участник с id {participant.Id}");
            }
            ParticipantLeft?.Invoke(this, new ParticipantLeftEventArgs(participant));
        }

        /// <summary>
        /// Потеряно соединение с сервером (клиентская сторона)
        /// </summary>
        private void OnTransportDisconnected()
        {
            _ = DisconnectAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.WriteLine($"Error during disconnect after transport loss: {t.Exception?.Message}");
                }

                ConnectionLost?.Invoke(this, new ConnectionLostEventArgs(
                    IsHost ? "Server stopped" : "Connection to room lost"));
            });
        }

        protected virtual void OnParticipantJoined(ParticipantJoinedEventArgs e) =>
            ParticipantJoined?.Invoke(this, e);

        protected virtual void OnParticipantLeft(ParticipantLeftEventArgs e) =>
            ParticipantLeft?.Invoke(this, e);

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e) =>
            MessageReceived?.Invoke(this, e);

        protected virtual void OnRoomStateChanged(RoomStateChangedEventArgs e) =>
            RoomStateChanged?.Invoke(this, e);

        protected virtual void OnConnectionLost(ConnectionLostEventArgs e) =>
            ConnectionLost?.Invoke(this, e);

        public async ValueTask DisposeAsync()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            await DisconnectAsync();

            if (server is IAsyncDisposable sd) await sd.DisposeAsync();
            if (client is IAsyncDisposable cd) await cd.DisposeAsync();
            if (discoveryClient is IAsyncDisposable dc) await dc.DisposeAsync();
            if (discoveryServer is IAsyncDisposable ds) await ds.DisposeAsync();
        }
    }
}