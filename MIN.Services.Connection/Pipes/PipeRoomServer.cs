using System.IO.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Services;

namespace MIN.Services.Connection.Pipes
{
    public sealed class PipeRoomServer : IPipeRoomServer
    {
        private readonly IPipeMessageSerializer serializer;
        private readonly List<Participant> connectedClients = new();

        private NamedPipeServerStream? pipe;
        private CancellationTokenSource? cancellationTokenSource;
        private Room? room;

        public event EventHandler<ChatMessage>? MessageReceived;
        public event EventHandler<Participant>? ParticipantJoined;
        public event EventHandler<Participant>? ParticipantLeft;
        public event EventHandler<Participant>? ClientDisconnected;

        public PipeRoomServer(IPipeMessageSerializer serializer)
        {
            this.serializer = serializer;
        }

        public bool IsRunning => pipe?.IsConnected == true && cancellationTokenSource?.IsCancellationRequested == false;

        public Room? Room => room;

        public async Task StartAsync(Room room, CancellationToken cancellationToken = default)
        {
            if (IsRunning) await StopAsync();

            this.room = room;
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var pipeName = PipeNameProvider.GetRoomPipeName(room.Id);
            pipe = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                room.MaximumParticipants,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough
            );

            // Отправляем информацию о комнате первому подключившемуся клиенту
            _ = AcceptClientAsync(cancellationTokenSource.Token);

            // Системное сообщение о создании комнаты
            var systemMsg = new ChatMessage
            {
                AsRoomMessage = true,
                Content = $"Room '{room.Name}' created by {room.HostParticipant.Name}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };
            room.AddMessage(systemMsg);
        }

        private async Task AcceptClientAsync(CancellationToken ct)
        {
            try
            {
                await pipe!.WaitForConnectionAsync(ct);

                // Запускаем обработку сообщений от клиента
                _ = ProcessClientMessagesAsync(ct);

                // Если есть место — создаём новую "запасную" трубу
                if (connectedClients.Count < room!.MaximumParticipants - 1)
                {
                    pipe = new NamedPipeServerStream(
                        PipeNameProvider.GetRoomPipeName(room.Id),
                        PipeDirection.InOut,
                        room.MaximumParticipants,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous | PipeOptions.WriteThrough
                    );

                    _ = AcceptClientAsync(ct);
                }
            }
            catch (OperationCanceledException) { /* Ожидаемое завершение */ }
        }

        private async Task SendRoomInfoAsync(CancellationToken ct)
        {
            // Отправляем RoomInfo как системное сообщение
            var roomInfo = new RoomInfoMessage
            {
                RoomId = room!.Id,
                RoomName = room.Name,
                MaxParticipants = room.MaximumParticipants,
                HostName = room.HostParticipant.Name,
                HostPCName = room.HostParticipant.PCName,
                CurrentParticipants = room.CurrentParticipants.Select(p =>
                    new Participant { Name = p.Name, PCName = p.PCName }).ToList()
            };

            await serializer.WriteMessageAsync(pipe!, roomInfo, ct);
        }

        private async Task ProcessClientMessagesAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested && pipe!.IsConnected)
                {
                    var message = await serializer.ReadMessageAsync(pipe!, ct);
                    switch (message)
                    {
                        case RoomInfoMessage roomInfo:
                            await SendRoomInfoAsync(ct);
                            break;

                        case ChatMessage chatMsg when chatMsg.MessageType == MessageType.System
                                 && chatMsg.Content.StartsWith("JOIN:"):
                            var participant = ParseParticipantFromMessage(chatMsg);
                            connectedClients.Add(participant);
                            ParticipantJoined?.Invoke(this, participant);
                            break;

                        case ChatMessage chatMsg when chatMsg.MessageType == MessageType.System
                                 && chatMsg.Content.StartsWith("LEAVE:"):
                            var sentParticipant = ParseParticipantFromMessage(chatMsg);
                            connectedClients.Remove(sentParticipant);
                            ParticipantLeft?.Invoke(this, sentParticipant);
                            break;

                        case ChatMessage chatMsg:
                            MessageReceived?.Invoke(this, chatMsg);
                            await BroadcastMessageAsync(chatMsg, ct);
                            break;

                        default:
                            // Неизвестный тип — игнорируем
                            break;
                    }
                }
            }
            catch
            {
                // Клиент отключился — удаляем из списка
                // (реальная реализация требует трекинга по уникальному идентификатору клиента)
            }
        }

        private async Task BroadcastMessageAsync(ChatMessage message, CancellationToken ct)
        {
            // В реальной реализации нужно отправлять всем подключённым клиентам
            // Здесь упрощённо — отправляем обратно в тот же поток (для демонстрации)
            await serializer.WriteMessageAsync(pipe!, message, ct);
        }

        private Participant ParseParticipantFromMessage(ChatMessage message)
        {
            var parts = message.Content.Split(':', 2);
            return new Participant
            {
                Name = parts.Length > 1 ? parts[1] : "Unknown",
                PCName = message.SenderPCName
            };
        }

        public async Task StopAsync()
        {
            cancellationTokenSource?.Cancel();
            if (pipe != null) await pipe.DisposeAsync();
            pipe = null;
            room = null;
            connectedClients.Clear();
        }

        public async Task SendMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            if (!IsRunning) throw new InvalidOperationException("Server not running");
            await serializer.WriteMessageAsync(pipe!, message, cancellationToken);
        }

        public async Task BroadcastSystemMessageAsync(string content, CancellationToken cancellationToken = default)
        {
            var systemMsg = new ChatMessage
            {
                AsRoomMessage = true,
                Content = content,
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };
            await SendMessageAsync(systemMsg, cancellationToken);
        }

        public async ValueTask DisposeAsync() => await StopAsync();
    }
}
