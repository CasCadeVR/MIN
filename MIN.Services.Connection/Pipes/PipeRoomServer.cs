using System.Diagnostics;
using System.IO.Pipes;
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

        public event EventHandler<ChatMessage>? MessageReceived;
        public event EventHandler<Participant>? ParticipantJoined;
        public event EventHandler<Participant>? ParticipantLeft;
        public event EventHandler<Participant>? ClientDisconnected;

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

            // Отправляем информацию о комнате первому подключившемуся клиенту
            for (int i = 0; i < room.MaximumParticipants; i++)
            {
                _ = AcceptClientAsync(cancellationTokenSource.Token);
            }

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
            while (!ct.IsCancellationRequested && IsRunning)
            {
                try
                {
                    // Создаем ОДНУ трубу для следующего клиента
                    var clientPipe = new NamedPipeServerStream(
                        PipeNameProvider.GetRoomPipeName(room!.Id),
                        PipeDirection.InOut,
                        room.MaximumParticipants,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous | PipeOptions.WriteThrough
                    );

                    await clientPipe.WaitForConnectionAsync(ct);

                    // Создаем подключение и сохраняем его
                    var connection = new ClientConnection(clientPipe);
                    lock (activeConnections)
                    {
                        activeConnections.Add(connection);
                    }

                    // Запускаем обработку в фоне
                    connection.ProcessingTask = HandleClientConnectionAsync(connection, ct)
                        .ContinueWith(t =>
                        {
                            // При завершении удаляем подключение
                            lock (activeConnections)
                            {
                                activeConnections.Remove(connection);
                            }
                            connection.DisposeAsync().AsTask().Wait();
                        });

                    // Создаем новую "запасную" трубу ТОЛЬКО если есть место
                    if (activeConnections.Count < room.MaximumParticipants)
                    {
                        _ = AcceptClientAsync(ct); // Рекурсивный вызов для следующего слота
                    }
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

        private async Task HandleClientConnectionAsync(ClientConnection connection, CancellationToken ct)
        {
            try
            {
                // Отправляем RoomInfo
                await SendRoomInfoAsync(connection.Pipe, ct);

                // Читаем JOIN сообщение
                var joinMessage = await serializer.ReadMessageAsync(connection.Pipe, ct);
                if (joinMessage is ChatMessage chatMsg &&
                    chatMsg.MessageType == MessageType.System &&
                    chatMsg.Content.StartsWith("JOIN:"))
                {
                    var participant = ParseParticipantFromMessage(chatMsg);
                    connection.Participant = participant;

                    lock (connectedClients)
                    {
                        connectedClients.Add(participant);
                        room.AddParticipant(participant);
                    }

                    ParticipantJoined?.Invoke(this, participant);
                }

                // Теперь читаем обычные сообщения
                while (!ct.IsCancellationRequested && connection.Pipe.IsConnected)
                {
                    var message = await serializer.ReadMessageAsync(connection.Pipe, ct);
                    if (message is ChatMessage chatMessage)
                    {
                        // Рассылаем ВСЕМ клиентам, кроме отправителя
                        await BroadcastMessageAsync(connection, chatMessage, ct);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Клиент отключился
                if (connection.Participant != null)
                {
                    ParticipantLeft?.Invoke(this, connection.Participant);
                    lock (connectedClients)
                    {
                        connectedClients.Remove(connection.Participant);
                    }
                }
            }
        }

        private async Task SendRoomInfoAsync(NamedPipeServerStream pipe, CancellationToken ct)
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

            await serializer.WriteMessageAsync(pipe, roomInfo, ct);
        }

        private async Task BroadcastMessageAsync(ClientConnection sender, ChatMessage message, CancellationToken ct)
        {
            var tasks = new List<Task>();

            lock (activeConnections)
            {
                foreach (var connection in activeConnections)
                {
                    if (connection != sender && connection.Pipe.IsConnected)
                    {
                        tasks.Add(serializer.WriteMessageAsync(connection.Pipe, message, ct));
                    }
                }
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            // Также уведомляем локальных подписчиков
            MessageReceived?.Invoke(this, message);
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

        public async Task SendMessageAsync(NamedPipeServerStream pipe, ChatMessage message, CancellationToken cancellationToken = default)
        {
            if (!IsRunning) throw new InvalidOperationException("Server not running");
            await serializer.WriteMessageAsync(pipe!, message, cancellationToken);
        }

        public async Task BroadcastSystemMessageAsync(NamedPipeServerStream pipe, string content, CancellationToken cancellationToken = default)
        {
            var systemMsg = new ChatMessage
            {
                AsRoomMessage = true,
                Content = content,
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };
            await SendMessageAsync(pipe, systemMsg, cancellationToken);
        }

        public async ValueTask DisposeAsync() => await StopAsync();
    }
}
