using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Interfaces.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Messages;
using MIN.Services.Extensions;
using MIN.Services.Services;

namespace MIN.Services.Connection.Pipes
{
    public sealed class PipeRoomServer : IPipeRoomServer, IAsyncDisposable
    {
        private readonly IPipeMessageSerializer serializer;
        private readonly ICryptoProvider cryptoProvider;
        private readonly ILoggerProvider logger;
        private readonly List<ClientConnection> activeConnections = new();

        private CancellationTokenSource? cancellationTokenSource;
        private Room? room;
        private bool IsRunning = false;

        public PipeRoomServer(IPipeMessageSerializer serializer, ICryptoProvider cryptoProvider, ILoggerProvider logger)
        {
            this.serializer = serializer;
            this.cryptoProvider = cryptoProvider;
            this.logger = logger;
        }

        /// <inheritdoc cref="IPipeRoomServer.Room"/>
        public Room? Room => room;
        bool IPipeRoomServer.IsRunning => IsRunning;

        public async Task StartAsync(Room room, CancellationToken cancellationToken = default)
        {
            if (IsRunning) await StopAsync();

            this.room = room.GetSerializableCopy();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            IsRunning = true;

            for (int i = 0; i < room.MaximumParticipants; i++)
            {
                _ = AcceptClientAsync(cancellationTokenSource.Token);
            }

            SendRoomCreatedInfo();
        }

        private void SendRoomCreatedInfo()
        {
            var systemMsg = new ChatMessage
            {
                Content = $"Комната '{room!.Name}' Была создана {room.HostParticipant.Name} в {TimeOnly.FromDateTime(DateTime.Now):t}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };
            room.AddMessage(systemMsg);
            logger.Log(systemMsg.Content);
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

                    // Client connected here

                    var connection = new ClientConnection(clientPipeSlot);
                    
                    connection.ProcessingTask = HandleClientConnectionAsync(connection, cancellationToken)
                        .ContinueWith(async (_) =>  await HandleClientConnectionLossAsync(connection, cancellationToken), cancellationToken);

                    CreateNewConnectionSlot(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    logger.Log($"Приём участников в комнате был оборван");
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
            if (IsRunning && activeConnections.Count < room!.MaximumParticipants)
            {
                _ = AcceptClientAsync(cancellationToken);
            }
        }

        private async Task HandleClientConnectionLossAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            if (connection.Participant != null)
            {
                logger.Log($"Участник {connection.Participant.Name} отключился от комнаты {room?.Name}");
                room?.RemoveParticipantById(connection.Participant.Id);
            }

            lock (activeConnections)
            {
                activeConnections.Remove(connection);
            }
            await connection.DisposeAsync();

            CreateNewConnectionSlot(cancellationToken);
        }

        private async Task HandleClientConnectionAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            lock (activeConnections)
            {
                activeConnections.Add(connection);
            }

            try
            {
                var firstMessage = await serializer.ReadMessageAsync(connection.Pipe, connection.Participant?.Id ?? Guid.Empty, cancellationToken);

                if (firstMessage is HandshakeMessage clientHandshake)
                {
                    await cryptoProvider.InitializeSessionAsync(clientHandshake.UserId, clientHandshake);
                    logger.Log($"Сессия с клиентом {clientHandshake.UserId} инициализирована");
                    connection.Participant!.Id = clientHandshake.UserId;

                    var serverHandshake = await cryptoProvider.CreateHandshakeAsync(room!.HostParticipant.Id);

                    logger.Log($"Сервер отправляет Handshake клиенту {clientHandshake.UserId}");
                    await serializer.WriteMessageAsync(connection.Pipe, serverHandshake, clientHandshake.UserId, cancellationToken);

                    var requestMessage = await serializer.ReadMessageAsync(connection.Pipe, clientHandshake.UserId, cancellationToken);

                    if (requestMessage is RoomInfoMessage)
                    {
                        logger.Log($"Получен RoomInfoRequest от {clientHandshake.UserId}. Отправляю RoomInfo...");
                        await SendRoomInfoAsync(connection, cancellationToken);
                    }
                    else
                    {
                        logger.Log($"Ожидали RoomInfoRequest, получили: {requestMessage?.GetType().Name}", LogLevel.Error);
                        return;
                    }
                }
                else
                {
                    logger.Log($"Клиент не отправил Handshake первым. Отключаем.", LogLevel.Error);
                    return;
                }

                while (!cancellationToken.IsCancellationRequested && connection.Pipe.IsConnected)
                {
                    var message = await serializer.ReadMessageAsync(connection.Pipe, connection.Participant.Id, cancellationToken);

                    switch (message)
                    {
                        case ChatMessage chatMessage when chatMessage.MessageType == MessageType.System
                                && chatMessage.Content.StartsWith("JOIN:"):
                            var joiningParticipant = ParseParticipantFromMessage(chatMessage);
                            connection.Participant = joiningParticipant;

                            if (connection.Participant.Id == room!.HostParticipant.Id)
                            {
                                room.HostParticipant.Name = joiningParticipant.Name;
                            }

                            room?.AddParticipant(joiningParticipant);
                            room!.AddMessage(chatMessage);

                            await BroadcastMessageAsync(connection, chatMessage, cancellationToken);
                            break;

                        case ChatMessage chatMessage when chatMessage.MessageType == MessageType.System
                                && chatMessage.Content.StartsWith("LEAVE:"):
                            var leavingParticipant = ParseParticipantFromMessage(chatMessage);

                            room?.RemoveParticipantById(leavingParticipant.Id);
                            room!.AddMessage(chatMessage);

                            await BroadcastMessageAsync(connection, chatMessage, cancellationToken);
                            break;

                        case ChatMessage chatMessage:
                            room!.AddMessage(chatMessage);
                            await BroadcastMessageAsync(connection, chatMessage, cancellationToken);
                            break;

                        default:
                            logger.Log("Сервер получил какое-то неизвестное сообщение");
                            continue;
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Client disconnected
                await HandleClientConnectionLossAsync(connection, cancellationToken);
            }
        }

        private async Task SendRoomInfoAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            var roomInfo = new RoomInfoMessage
            {
                RoomId = room!.Id,
                RoomName = room.Name,
                MaxParticipants = room.MaximumParticipants,
                HostName = room.HostParticipant.Name,
                HostPCName = room.HostParticipant.PCName,
                CurrentParticipants = room.CurrentParticipants.Select(p => p.GetSerializableCopy()).ToList(),
                ChatHistory = room.ChatHistory.Select(m => m.GetSerializableCopy()).ToList(),
            };

            await serializer.WriteMessageAsync(connection.Pipe, roomInfo, connection.Participant.Id, cancellationToken);
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
                        tasks.Add(serializer.WriteMessageAsync(connection.Pipe, message, connection.Participant.Id, ct));
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

        public async Task SendMessageAsync(ClientConnection connection, ChatMessage message, CancellationToken cancellationToken = default)
        {
            if (!IsRunning) throw new InvalidOperationException("Сервер не работает");
            await serializer.WriteMessageAsync(connection.Pipe, message, connection.Participant.Id, cancellationToken);
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
        }

        public async ValueTask DisposeAsync() => await StopAsync();
    }
}
