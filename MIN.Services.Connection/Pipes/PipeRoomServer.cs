using System.Diagnostics;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Services.Connection.Contracts.Interfaces.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Services;

namespace MIN.Services.Connection.Pipes
{
    public sealed class PipeRoomServer : IPipeRoomServer, IAsyncDisposable
    {
        private readonly IPipeMessageSerializer serializer;
        private readonly List<Participant> connectedClients = new();
        private readonly List<ClientConnection> activeConnections = new();

        private CancellationTokenSource? cancellationTokenSource;
        private Room? room;

        public PipeRoomServer(IPipeMessageSerializer serializer)
        {
            this.serializer = serializer;
        }

        bool IPipeRoomServer.IsRunning => IsRunning;

        private bool IsRunning = false;

        public Room? Room => room;

        public async Task StartAsync(Room room, CancellationToken cancellationToken = default)
        {
            if (IsRunning) await StopAsync();

            this.room = room;
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            IsRunning = true;

            for (int i = 0; i < room.MaximumParticipants; i++)
            {
                _ = AcceptClientAsync(cancellationTokenSource.Token);
            }

            var systemMsg = new ChatMessage
            {
                Content = $"Комната '{room.Name}' Была создана {room.HostParticipant.Name} в {TimeOnly.FromDateTime(DateTime.Now).ToShortTimeString()}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };
            room.AddMessage(systemMsg);
        }

        private async Task AcceptClientAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && IsRunning)
            {
                try
                {
                    if (!OperatingSystem.IsWindows())
                    {
                        throw new PlatformNotSupportedException("К сожалению, пока только на Windows");
                    }

                    var securityIdentifier = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                    var pipeSecurity = new PipeSecurity();
                    pipeSecurity.AddAccessRule(
                        new PipeAccessRule(securityIdentifier,
                            PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
                    AccessControlType.Allow));

                    var clientPipeSlot = NamedPipeServerStreamAcl.Create(
                        PipeNameProvider.GetRoomPipeName(room!.Id),
                        PipeDirection.InOut,
                        room.MaximumParticipants,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                        0, 0,
                        pipeSecurity);

                    await clientPipeSlot.WaitForConnectionAsync(cancellationToken);

                    // Создаем подключение и сохраняем его
                    var connection = new ClientConnection(clientPipeSlot);
                    lock (activeConnections)
                    {
                        activeConnections.Add(connection);
                    }

                    // Запускаем обработку в фоне
                    connection.ProcessingTask = HandleClientConnectionAsync(connection, cancellationToken)
                        .ContinueWith(t =>
                        {
                            lock (activeConnections)
                            {
                                activeConnections.Remove(connection);
                            }
                            connection.DisposeAsync().AsTask().Wait();
                            CreateNewConnectionSlot(cancellationToken);
                        });

                    CreateNewConnectionSlot(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Canceling in AcceptClientAsync");
                    break;
                }
                catch (IOException ex) when (ex.Message.Contains("pipe is being closed"))
                {
                    continue;
                }
            }
        }

        private void CreateNewConnectionSlot(CancellationToken cancellationToken)
        {
            if (!IsRunning)
            {
                return;
            }

            if (activeConnections.Count < room!.MaximumParticipants)
            {
                _ = AcceptClientAsync(cancellationToken);
            }
        }

        private async Task HandleClientConnectionAsync(ClientConnection connection, CancellationToken ct)
        {
            try
            {
                await SendRoomInfoAsync(connection.Pipe, ct);

                while (!ct.IsCancellationRequested && connection.Pipe.IsConnected)
                {
                    var message = await serializer.ReadMessageAsync(connection.Pipe, ct);

                    switch (message)
                    {
                        case ChatMessage chatMessage when chatMessage.MessageType == MessageType.System
                                && chatMessage.Content.StartsWith("JOIN:"):
                            var joiningParticipant = ParseParticipantFromMessage(chatMessage);
                            connection.Participant = joiningParticipant;

                            room?.AddParticipant(joiningParticipant);

                            lock (connectedClients)
                            {
                                connectedClients.Add(joiningParticipant);
                            }

                            await BroadcastMessageAsync(connection, chatMessage, ct);
                            break;

                        case ChatMessage chatMessage when chatMessage.MessageType == MessageType.System
                                && chatMessage.Content.StartsWith("LEAVE:"):
                            var leavingParticipant = ParseParticipantFromMessage(chatMessage);

                            if (room?.RemoveParticipantById(leavingParticipant.Id) == false)
                            {
                                throw new ArgumentNullException($"Не нашёлся участник с id {leavingParticipant.Id}");
                            }

                            lock (connectedClients)
                            {
                                connectedClients.Remove(leavingParticipant);
                            }

                            await BroadcastMessageAsync(connection, chatMessage, ct);
                            break;

                        case ChatMessage chatMessage:
                            await BroadcastMessageAsync(connection, chatMessage, ct);
                            break;

                        default:
                            // Неизвестный тип — игнорируем
                            break;
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Client disconnected
                if (connection.Participant != null)
                {
                    room?.RemoveParticipantById(connection.Participant.Id);
                    lock (connectedClients)
                    {
                        connectedClients.Remove(connection.Participant);
                    }
                }
            }
        }

        private async Task SendRoomInfoAsync(NamedPipeServerStream pipe, CancellationToken ct)
        {
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

            await serializer.WriteMessageAsync(pipe, roomInfo, ct);
        }

        public async Task BroadcastMessageAsync(ClientConnection sender, ChatMessage message, CancellationToken ct)
        {
            var tasks = new List<Task>();

            lock (activeConnections)
            {
                foreach (var connection in activeConnections)
                {
                    if (connection.Pipe.IsConnected)
                    {
                        tasks.Add(serializer.WriteMessageAsync(connection.Pipe, message, ct));
                    }
                }
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }
        }

        private Participant ParseParticipantFromMessage(ChatMessage message)
        {
            var parts = message.Content.Split(':', 3);
            var name = parts.Length > 1 ? parts[1] : "Unknown";
            var id = parts.Length > 2 ? Guid.Parse(parts[2]) : Guid.NewGuid();

            return new Participant
            {
                Id = id,
                Name = name,
                PCName = message.SenderPCName
            };
        }

        public async Task StopAsync()
        {
            IsRunning = false;
            cancellationTokenSource?.Cancel();

            // Очистка подключений
            lock (activeConnections)
            {
                foreach (var connection in activeConnections.ToList())
                {
                    connection.DisposeAsync();
                }
                activeConnections.Clear();
            }

            room = null;
            connectedClients.Clear();
        }

        public async Task SendMessageAsync(ClientConnection connection, ChatMessage message, CancellationToken cancellationToken = default)
        {
            if (!IsRunning) throw new InvalidOperationException("Сервер не работает");
            await serializer.WriteMessageAsync(connection.Pipe, message, cancellationToken);
        }

        public async ValueTask DisposeAsync() => await StopAsync();
    }
}
