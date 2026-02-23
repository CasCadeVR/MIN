using System.IO.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Interfaces.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Messages;
using MIN.Services.Services;

namespace MIN.Services.Connection.Pipes
{
    /// <inheritdoc cref="IPipeParticipantClient"/>
    public sealed class PipeParticipantClient : IPipeParticipantClient
    {
        private readonly IPipeMessageSerializer serializer;
        private readonly ICryptoProvider cryptoProvider;
        private readonly ILoggerProvider logger;

        private NamedPipeClientStream? pipe;
        private CancellationTokenSource? cancellationTokenSource;
        private Room? currentRoom;
        private Participant? selfParticipant;
        private bool isDisposed;
        private Guid roomHostParticipantId;

        public event EventHandler<ChatMessage>? MessageReceived;
        public event EventHandler<RoomInfoMessage>? RoomInfoReceived;
        public event EventHandler<Participant>? ParticipantJoined;
        public event EventHandler<Participant>? ParticipantLeft;
        public event EventHandler? Disconnected;

        public bool IsConnected => pipe?.IsConnected == true && !isDisposed;

        public Room? Room => currentRoom;

        public PipeParticipantClient(IPipeMessageSerializer serializer, ICryptoProvider cryptoProvider, ILoggerProvider logger)
        {
            this.serializer = serializer;
            this.cryptoProvider = cryptoProvider;
            this.logger = logger;
            roomHostParticipantId = Guid.Empty;
        }

        /// <summary>
        /// Подключиться к существующей комнате
        /// </summary>
        public async Task ConnectAsync(Room room, Participant selfParticipant, int timeoutMs = 1000, CancellationToken cancellationToken = default)
        {
            if (IsConnected) await DisconnectAsync();

            roomHostParticipantId = room.HostParticipant.Id;

            var roomId = room.Id;

            if (!PipeNameProvider.IsValidPipeName(roomId.ToString()))
                throw new ArgumentException("Invalid room ID", nameof(roomId));

            isDisposed = false;

            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            this.selfParticipant = selfParticipant;

            var pipeName = PipeNameProvider.GetRoomPipeName(roomId);
            pipe = new NamedPipeClientStream(
                room.HostParticipant.PCName,
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough
            );

            try
            {
                logger.Log($"Подключаюсь к комнате {room.Name} c id {roomId}");
                await pipe.ConnectAsync(timeoutMs, cancellationTokenSource.Token);

                await SendHandshakeMessageAsync(cancellationTokenSource.Token);
                _ = ReceiveMessagesAsync(cancellationTokenSource.Token);
                await SendJoinNotificationAsync(cancellationTokenSource.Token);
            }
            catch (TimeoutException)
            {
                await DisconnectAsync(cancellationTokenSource.Token);
                logger.Log($"Не получилось подсоединиться к комнате '{room.Name}'. Походу либо заполнена, либо она исчезла.", LogLevel.Error);
                throw new TimeoutException($"Не получилось подсоединиться к комнате '{room.Name}'. Походу либо заполнена, либо она исчезла.");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                await DisconnectAsync(cancellationTokenSource.Token);
                logger.Log($"Не получилось подсоединиться к комнате '{room.Name}'. Походу либо заполнена, либо она исчезла.", LogLevel.Warning);
                throw new InvalidOperationException($"Соединение не удалось: {ex.Message}");
            }
        }

        /// <summary>
        /// Фоновое чтение всех входящих сообщений
        /// </summary>
        private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = await serializer.ReadMessageAsync(pipe!, selfParticipant!.Id, cancellationToken);

                    switch (message)
                    {
                        case HandshakeMessage serverHandshake:
                            logger.Log($"Получен Handshake от сервера: {serverHandshake.UserId}");
                            await cryptoProvider.InitializeSessionAsync(serverHandshake.UserId, serverHandshake);
                            logger.Log($"Сессия с сервером {serverHandshake.UserId} инициализирована");
                            break;

                        case RoomInfoMessage roomInfo:
                            logger.Log($"Получена RoomInfo: {roomInfo.RoomName}");
                            await HandleRoomInfoAsync(roomInfo, cancellationToken);
                            break;

                        case ChatMessage chatMsg when chatMsg.MessageType == MessageType.System
                                && chatMsg.Content.StartsWith("JOIN:"):
                            HandleParticipantJoined(chatMsg);
                            break;

                        case ChatMessage chatMsg when chatMsg.MessageType == MessageType.System
                                && chatMsg.Content.StartsWith("LEAVE:"):
                            HandleParticipantLeft(chatMsg);
                            break;

                        case ChatMessage chatMsg:
                            HandleChatMessage(chatMsg);
                            break;

                        default:
                            // Неизвестный тип — игнорируем
                            break;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                logger.Log("Я прекратил получать сообщения от сервера");
            }
            catch (IOException ex) when (!isDisposed)
            {
                logger.Log($"Я отключился от сервера: {ex.Message}");
                await DisconnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка у клиента: {ex.Message}", LogLevel.Error);
                await DisconnectAsync(cancellationToken);
            }
        }

        private async Task HandleRoomInfoAsync(RoomInfoMessage roomInfo, CancellationToken ct)
        {
            currentRoom = new Room(roomInfo.RoomName, roomInfo.MaxParticipants)
            {
                Id = roomInfo.RoomId,
                HostParticipant = new Participant
                {
                    Id = roomInfo.HostId,
                    Name = roomInfo.HostName,
                    PCName = roomInfo.HostPCName
                }
            };

            foreach (var participant in roomInfo.CurrentParticipants)
            {
                currentRoom.AddParticipant(participant);
            }

            RoomInfoReceived?.Invoke(this, roomInfo);
        }

        private void HandleParticipantJoined(ChatMessage systemMessage)
        {
            var participant = ParseParticipantFromSystemMessage(systemMessage);
            currentRoom?.AddParticipant(participant);
            ParticipantJoined?.Invoke(this, participant);
        }

        private void HandleParticipantLeft(ChatMessage systemMessage)
        {
            var participant = ParseParticipantFromSystemMessage(systemMessage);
            if (currentRoom?.RemoveParticipantById(participant.Id) == false)
            {
                throw new ArgumentNullException($"Не нашёлся участник с id {participant.Id}");
            }
            ParticipantLeft?.Invoke(this, participant);
        }

        private void HandleChatMessage(ChatMessage message)
        {
            if (message.SenderPCName == selfParticipant?.PCName && message.Time == default)
            {
                message.Time = TimeOnly.FromDateTime(DateTime.Now);
            }

            MessageReceived?.Invoke(this, message);
        }

        private Participant ParseParticipantFromSystemMessage(ChatMessage systemMessage)
        {
            var parts = systemMessage.Content.Split(':', 3);
            var name = parts.Length > 1 ? parts[1] : "Unknown";
            var id = parts.Length > 2 ? Guid.Parse(parts[2]) : Guid.NewGuid();

            return new Participant
            {
                Id = id,
                Name = name,
                PCName = systemMessage.SenderPCName
            };
        }

        private async Task SendHandshakeMessageAsync(CancellationToken cancellationToken)
        {
            if (selfParticipant == null || pipe == null || !pipe.IsConnected)
            {
                return;
            }

            var handShakeMessage = await cryptoProvider.CreateHandshakeAsync(selfParticipant.Id);

            try
            {
                await serializer.WriteMessageAsync(pipe, handShakeMessage, roomHostParticipantId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка у клиента в отправке Handshake сообщения: {ex.Message}", LogLevel.Error);
            }
        }

        private async Task SendJoinNotificationAsync(CancellationToken cancellationToken)
        {
            if (selfParticipant == null || pipe == null || !pipe.IsConnected)
            {
                return;
            }

            var joinMessage = new ChatMessage
            {
                SenderName = selfParticipant.Name,
                SenderPCName = selfParticipant.PCName,
                Content = $"JOIN:{selfParticipant.Name}:{selfParticipant.Id}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };

            try
            {
                await serializer.WriteMessageAsync(pipe, joinMessage, roomHostParticipantId, cancellationToken);
            }
            catch(Exception ex)
            {
                logger.Log($"Ошибка у клиента в отправке JOIN сообщения: {ex.Message}", LogLevel.Error);
            }
        }

        private async Task SendLeaveNotificationAsync(CancellationToken cancellationToken)
        {
            if (selfParticipant == null || pipe == null || !pipe.IsConnected)
                return;

            var leaveMessage = new ChatMessage
            {
                SenderName = selfParticipant.Name,
                SenderPCName = selfParticipant.PCName,
                Content = $"LEAVE:{selfParticipant.Name}:{selfParticipant.Id}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };

            try
            {
                await serializer.WriteMessageAsync(pipe, leaveMessage, roomHostParticipantId, cancellationToken);
            }
            catch
            {
                // Игнорируем ошибки при отправке LEAVE (сервер мог уже отключиться)
            }
        }

        /// <summary>
        /// Отправить сообщение в комнату
        /// </summary>
        public async Task SendMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || pipe == null || !pipe.IsConnected)
            {
                throw new InvalidOperationException("Не подключен не к одной комнате");
            }

            if (selfParticipant == null)
            {
                throw new InvalidOperationException("Участник не задан");
            }

            // Заполняем метаданные отправителя
            message.SenderName ??= selfParticipant.Name;
            message.SenderPCName ??= selfParticipant.PCName;
            message.Time = TimeOnly.FromDateTime(DateTime.Now); // Локальное время для отображения

            await serializer.WriteMessageAsync(pipe, message, roomHostParticipantId, cancellationToken);
        }

        /// <summary>
        /// Отключиться от комнаты
        /// </summary>
        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (isDisposed || pipe == null || !pipe.IsConnected)
                return;

            isDisposed = true;
            cancellationTokenSource?.Cancel();

            // Отправляем уведомление о выходе (если ещё можем)
            if (pipe.IsConnected && selfParticipant != null)
            {
                try
                {
                    await SendLeaveNotificationAsync(CancellationToken.None);
                }
                catch { /* Игнорируем ошибки при отправке LEAVE */ }
            }

            // Очищаем состояние
            await pipe.DisposeAsync();
            pipe = null;
            currentRoom = null;
            selfParticipant = null;

            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public async ValueTask DisposeAsync()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                await DisconnectAsync();
                cancellationTokenSource?.Dispose();
            }
        }
    }
}
