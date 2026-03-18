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
using MIN.Services.Contracts.Models.Messages;
using MIN.Services.Extensions;

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
        private readonly ILoggerProvider logger;

        // Текущее состояние (только одно активное подключение)
        private Room? currentRoom;
        private Participant? selfParticipant;
        private CancellationTokenSource? cancellationTokenSource = new();
        private bool isDisposed;
        private IDiscoveryServer discoveryServer = null!;
        private IDiscoveryClient discoveryClient = null!;

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
            IPipeMessageSerializer serializer,
            ILoggerProvider logger)
        {
            this.server = server;
            this.client = client;
            this.serializer = serializer;
            this.logger = logger;

            client.MessageReceived += (s, e) => OnTransportMessageReceived(e);
            client.RoomInfoReceived += (s, e) => OnRoomInfoReceived(e);
            client.ParticipantJoined += async (s, e) => await OnTransportParticipantJoined(e);
            client.ParticipantLeft += (s, e) => OnTransportParticipantLeft(e);
            client.Disconnected += (s, e) => OnTransportDisconnected();
        }

        async IAsyncEnumerable<DiscoveredRoom> IChatRoomService.DiscoverAvailableRoomsAsync(IEnumerable<string> targetPCNames, int timeoutMs, CancellationToken cancellationToken)
        {
            var roomDiscoveringTasks = new List<RoomDiscoveringTask>();
            discoveryClient = new DiscoveryClient(serializer, logger);

            foreach (var pcName in targetPCNames)
            {
                logger.Log($"Пытаюсь достучаться до компьютера: {pcName}...", LogLevel.Information);
                var roomDiscoveringTask = new RoomDiscoveringTask
                {
                    PcName = pcName,
                    Task = discoveryClient.DiscoverRoomAsync(pcName, TimeSpan.FromMilliseconds(timeoutMs), cancellationToken)
                };
                roomDiscoveringTasks.Add(roomDiscoveringTask);
            }

            while (roomDiscoveringTasks.Count > 0)
            {
                var firstFinished = await Task.WhenAny(roomDiscoveringTasks.Select(t => t.Task));
                var finishedTask = roomDiscoveringTasks.First(t => t.Task == firstFinished);

                roomDiscoveringTasks.Remove(finishedTask);

                DiscoveredRoom? room = null;
                try
                {
                    room = await firstFinished;
                }
                catch (RoomDiscoveryException ex)
                {
                    logger.Log($"Не удалось достучаться до компьютера: {finishedTask.PcName}: {ex.Message}", LogLevel.Information);
                }

                if (room != null)
                {
                    yield return room;
                }
            }
        }

        async Task<Room> IChatRoomService.CreateRoomAsync(string roomName, int maxParticipants, Participant host, CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(isDisposed, nameof(ChatRoomService));

            await DisconnectAsync(cancellationToken);

            currentRoom = new Room(roomName, maxParticipants) { HostParticipant = host };
            selfParticipant = host;

            await server.StartAsync(currentRoom, cancellationToken);
            return currentRoom;
        }

        async Task IChatRoomService.JoinRoomAsync(Room room, Participant participant, int timeoutMs, CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(isDisposed, nameof(ChatRoomService));

            selfParticipant = participant;
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await client.ConnectAsync(room, selfParticipant, timeoutMs, cancellationToken);
        }

        /// <inheritdoc cref="IPipeParticipantClient.GetUpdatedRoomInfoAsync(CancellationToken)"/>
        async Task IChatRoomService.GetUpdatedRoomInfoAsync(CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(isDisposed, nameof(ChatRoomService));

            if (currentRoom == null || selfParticipant == null)
            {
                throw new InvalidOperationException("Not connected to any room");
            }

            await client.GetUpdatedRoomInfoAsync(cancellationToken);
        }

        /// <inheritdoc cref="IPipeParticipantClient.SendUpdatedRoomRequestAsync(RoomInfoRequestMessage, CancellationToken)"/>
        public async Task SendUpdateRoomRequestAsync(RoomInfoRequestMessage request, CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(isDisposed, nameof(ChatRoomService));

            if (currentRoom == null || selfParticipant == null)
            {
                throw new InvalidOperationException("Not connected to any room");
            }

            await client.SendUpdatedRoomRequestAsync(request, cancellationToken);
        }

        async Task IChatRoomService.SendMessageAsync(string content, MessageType type, CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(isDisposed, nameof(ChatRoomService));

            if (currentRoom == null || selfParticipant == null)
            {
                throw new InvalidOperationException("Not connected to any room");
            }

            var message = new ChatMessage
            {
                Content = content,
                MessageType = type,
            };

            await client.SendMessageAsync(message, cancellationToken);
        }

        /// <summary>
        /// Отключиться от комнаты
        /// </summary>
        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (isDisposed)
            {
                return;
            }

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
        private void OnRoomInfoReceived(Room room)
        {
            currentRoom = room.GetSerializableCopy();
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
        private async Task OnTransportParticipantJoined(Participant participant)
        {
            currentRoom?.AddParticipant(participant);

            if (IsHost)
            {
                discoveryServer = new DiscoveryServer(participant, server.Room, serializer, logger);
                await discoveryServer.StartAsync(cancellationTokenSource!.Token);
            }

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
                    logger.Log($"Произошла ошибка во время потери соединения: {t.Exception?.Message}", LogLevel.Error);
                }

                ConnectionLost?.Invoke(this, new ConnectionLostEventArgs(
                    IsHost ? "Сервер был остановлен хостом" : "Соединение к комнате потеряно"));
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
            {
                return;
            }

            isDisposed = true;
            await DisconnectAsync();

            if (server is IAsyncDisposable sd)
            {
                await sd.DisposeAsync();
            }

            if (client is IAsyncDisposable cd)
            {
                await cd.DisposeAsync();
            }

            if (discoveryClient is IAsyncDisposable dc)
            {
                await dc.DisposeAsync();
            }

            if (discoveryServer is IAsyncDisposable ds)
            {
                await ds.DisposeAsync();
            }
        }
    }
}
