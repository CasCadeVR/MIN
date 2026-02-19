using System.Diagnostics;
using System.IO.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Services;

namespace MIN.Services.Connection.Pipes
{
    /// <inheritdoc cref="IPipeParticipantClient"/>
    public sealed class PipeParticipantClient : IPipeParticipantClient
    {
        private readonly IPipeMessageSerializer serializer;

        private NamedPipeClientStream? pipe;
        private CancellationTokenSource? cancellationTokenSource;
        private Room? currentRoom;
        private Participant? selfParticipant;
        private bool isDisposed;

        public event EventHandler<ChatMessage>? MessageReceived;
        public event EventHandler<RoomInfoMessage>? RoomInfoReceived;
        public event EventHandler<Participant>? ParticipantJoined;
        public event EventHandler<Participant>? ParticipantLeft;
        public event EventHandler? Disconnected;

        public bool IsConnected => pipe?.IsConnected == true && !isDisposed;

        public Room? Room => currentRoom;

        public PipeParticipantClient(IPipeMessageSerializer serializer)
        {
            this.serializer = serializer;
        }

        /// <summary>
        /// Подключиться к существующей комнате
        /// </summary>
        public async Task ConnectAsync(Room room, Participant selfParticipant, int timeoutMs = 1000, CancellationToken cancellationToken = default)
        {
            if (IsConnected) await DisconnectAsync();

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
                Debug.WriteLine($"Connecting to room: {roomId}");
                await pipe.ConnectAsync(timeoutMs, cancellationTokenSource.Token);

                // Client connected at this postion

                _ = ReceiveMessagesAsync(cancellationTokenSource.Token);
                await SendJoinNotificationAsync(cancellationTokenSource.Token);
            }
            catch (TimeoutException)
            {
                await DisconnectAsync(cancellationTokenSource.Token);
                throw new TimeoutException($"Could not connect to room '{roomId}'. Room may not exist or is full.");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                await DisconnectAsync(cancellationTokenSource.Token);
                throw new InvalidOperationException($"Connection failed: {ex.Message}");
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
                    var message = await serializer.ReadMessageAsync(pipe!, cancellationToken);

                    switch (message)
                    {
                        case RoomInfoMessage roomInfo:
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
                Debug.WriteLine("Stopped recieving messages");
            }
            catch (IOException ex) when (!isDisposed)
            {
                Debug.WriteLine($"Disconnected from server: {ex.Message}");
                await DisconnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PipeClient error: {ex.Message}");
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
            currentRoom?.RemoveParticipant(participant);
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
            var parts = systemMessage.Content.Split(':', 2);
            var name = parts.Length > 1 ? parts[1] : "Unknown";

            return new Participant
            {
                Name = name,
                PCName = systemMessage.SenderPCName
            };
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
                Content = $"JOIN:{selfParticipant.Name}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };

            try
            {
                await serializer.WriteMessageAsync(pipe, joinMessage, cancellationToken);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Error in sending Join Message: {ex.Message}");
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
                Content = $"LEAVE:{selfParticipant.Name}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };

            try
            {
                await serializer.WriteMessageAsync(pipe, leaveMessage, cancellationToken);
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
                throw new InvalidOperationException("Not connected to any room");

            if (selfParticipant == null)
                throw new InvalidOperationException("Self participant not set");

            // Заполняем метаданные отправителя
            message.SenderName ??= selfParticipant.Name;
            message.SenderPCName ??= selfParticipant.PCName;
            message.Time = TimeOnly.FromDateTime(DateTime.Now); // Локальное время для отображения

            await serializer.WriteMessageAsync(pipe, message, cancellationToken);
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
