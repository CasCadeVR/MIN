using System.Diagnostics;
using MIN.Services.Connection.Contracts.Interfaces;
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

        // Текущее состояние (только одно активное подключение)
        private Room? currentRoom;
        private Participant? selfParticipant;
        private bool isHost;
        private bool isDisposed;

        // События для подписки UI
        public event EventHandler<ParticipantJoinedEventArgs>? ParticipantJoined;
        public event EventHandler<ParticipantLeftEventArgs>? ParticipantLeft;
        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
        public event EventHandler<RoomStateChangedEventArgs>? RoomStateChanged;
        public event EventHandler<ConnectionLostEventArgs>? ConnectionLost;

        public ChatRoomService(IPipeRoomServer server, IPipeParticipantClient client)
        {
            this.server = server;
            this.client = client;

            // Подписка на события КЛИЕНТА
            this.client.MessageReceived += (s, e) => OnTransportMessageReceived(e);
            this.client.RoomInfoReceived += (s, e) => OnRoomInfoReceived(e);
            this.client.ParticipantJoined += (s, e) => OnTransportParticipantJoined(e);
            this.client.ParticipantLeft += (s, e) => OnTransportParticipantLeft(e);
            this.client.Disconnected += (s, e) => OnTransportDisconnected();

            // Подписка на события СЕРВЕРА (когда мы хост)
            this.server.MessageReceived += (s, e) => OnTransportMessageReceived(e);
            this.server.ParticipantJoined += (s, e) => OnTransportParticipantJoined(e);
            this.server.ParticipantLeft += (s, e) => OnTransportParticipantLeft(e);
            this.server.ClientDisconnected += (s, e) => OnServerClientDisconnected(e);
        }

        /// <summary>
        /// Создать новую комнату и стать хостом
        /// </summary>
        public async Task CreateRoomAsync(string roomName, int maxParticipants, Participant host)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ChatRoomService));

            await DisconnectAsync();

            currentRoom = new Room(roomName, maxParticipants) { HostParticipant = host };
            selfParticipant = host;
            isHost = true;

            // Добавляем себя как участника (хост тоже участник)
            currentRoom.AddParticipant(host);

            await server.StartAsync(currentRoom);
            OnRoomStateChanged(new RoomStateChangedEventArgs(currentRoom, RoomState.Created));
        }

        /// <summary>
        /// Подключиться к существующей комнате
        /// </summary>
        public async Task JoinRoomAsync(Guid roomId, Participant participant)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ChatRoomService));

            await DisconnectAsync();

            selfParticipant = participant;
            isHost = false;

            await client.ConnectAsync(roomId, participant);
            // Room будет получена из первого системного сообщения от сервера (RoomInfo)
            OnRoomStateChanged(new RoomStateChangedEventArgs(null, RoomState.Joined));
        }

        /// <summary>
        /// Отправить сообщение в текущую комнату
        /// </summary>
        public async Task SendMessageAsync(string content, MessageType type = MessageType.Text)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ChatRoomService));

            if (currentRoom == null || selfParticipant == null)
                throw new InvalidOperationException("Not connected to any room");

            var message = new ChatMessage
            {
                SenderName = selfParticipant.Name,
                SenderPCName = selfParticipant.PCName,
                Content = content,
                MessageType = type,
                Time = TimeOnly.FromDateTime(DateTime.Now), // Локальное время для отображения
                TimestampUtc = DateTime.UtcNow
            };

            if (isHost)
                await server.SendMessageAsync(message);
            else
                await client.SendMessageAsync(message);

            // Добавляем в локальную историю сразу (оптимистичное обновление)
            currentRoom.AddMessage(message);
            OnMessageReceived(new MessageReceivedEventArgs(message));
        }

        /// <summary>
        /// Отключиться от комнаты
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (isDisposed)
                return;

            try
            {
                if (isHost && server.IsRunning)
                    await server.StopAsync();
                else if (!isHost && client.IsConnected)
                    await client.DisconnectAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during disconnect: {ex.Message}");
            }
            finally
            {
                currentRoom = null;
                selfParticipant = null;
                isHost = false;

                OnRoomStateChanged(new RoomStateChangedEventArgs(null, RoomState.Disconnected));
            }
        }

        // === Обработчики событий от транспорта ===

        /// <summary>
        /// Получена информация о комнате от сервера (при подключении клиента)
        /// </summary>
        private void OnRoomInfoReceived(RoomInfoMessage roomInfo)
        {
            currentRoom = new Room(roomInfo.RoomName, roomInfo.MaxParticipants)
            {
                Id = roomInfo.RoomId,
                HostParticipant = new Participant() {
                    Name = roomInfo.HostName,
                    PCName = roomInfo.HostPCName,
                }
            };

            // Копируем текущих участников
            foreach (var participant in roomInfo.CurrentParticipants)
            {
                currentRoom.CurrentParticipants.Add(participant);
            }

            // Добавляем себя в список участников (локально)
            if (selfParticipant != null &&
                !currentRoom.CurrentParticipants.Any(p => p.PCName == selfParticipant.PCName))
            {
                currentRoom.CurrentParticipants.Add(selfParticipant);
            }

            OnRoomStateChanged(new RoomStateChangedEventArgs(currentRoom, RoomState.Joined));
        }

        /// <summary>
        /// Получено обычное сообщение от транспорта
        /// </summary>
        private void OnTransportMessageReceived(ChatMessage message)
        {
            if (message.MessageType != MessageType.System &&
                message.MessageType != MessageType.Command)
            {
                currentRoom?.AddMessage(message);
            }

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
            currentRoom?.RemoveParticipant(participant);
            ParticipantLeft?.Invoke(this, new ParticipantLeftEventArgs(participant));
        }

        /// <summary>
        /// Клиент отключился от сервера (событие на стороне хоста)
        /// </summary>
        private void OnServerClientDisconnected(Participant participant)
        {
            // Удаляем участника из локального списка
            currentRoom?.RemoveParticipant(participant);
            ParticipantLeft?.Invoke(this, new ParticipantLeftEventArgs(participant));

            // Если это был последний участник кроме хоста — комната пуста
            if (currentRoom != null && currentRoom.CurrentParticipants.Count == 1)
            {
                Debug.WriteLine("Room is now empty (only host remains)");
            }
        }

        /// <summary>
        /// Потеряно соединение с сервером (клиентская сторона)
        /// </summary>
        private void OnTransportDisconnected()
        {
            // Автоматически инициируем отключение для очистки состояния
            _ = DisconnectAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.WriteLine($"Error during disconnect after transport loss: {t.Exception?.Message}");
                }

                // Уведомляем UI о потере соединения
                ConnectionLost?.Invoke(this, new ConnectionLostEventArgs(
                    isHost ? "Server stopped" : "Connection to room lost"));
            });
        }

        // === Вспомогательные методы для вызова событий ===

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

        // === IDisposable ===

        public async ValueTask DisposeAsync()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            await DisconnectAsync();

            if (server is IAsyncDisposable sd) await sd.DisposeAsync();
            if (client is IAsyncDisposable cd) await cd.DisposeAsync();
        }
    }
}