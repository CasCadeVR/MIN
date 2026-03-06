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
        private bool isRunning = false;

        public PipeRoomServer(IPipeMessageSerializer serializer, ICryptoProvider cryptoProvider, ILoggerProvider logger)
        {
            this.serializer = serializer;
            this.cryptoProvider = cryptoProvider;
            this.logger = logger;
        }

        /// <inheritdoc cref="IPipeRoomServer.Room"/>
        public Room? Room => room;
        bool IPipeRoomServer.IsRunning => isRunning;

        public async Task StartAsync(Room room, CancellationToken cancellationToken = default)
        {
            if (isRunning)
            {
                await StopAsync();
            }

            this.room = room.GetSerializableCopy();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            isRunning = true;

            SendRoomCreatedInfo();

            _ = AcceptClientAsync(cancellationTokenSource.Token);
        }

        private void SendRoomCreatedInfo()
        {
            var systemMsg = new ChatMessage
            {
                Content = $"Комната {room!.Name} Была создана в {TimeOnly.FromDateTime(DateTime.Now):t}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };
            room.AddMessage(systemMsg);
            logger.Log(systemMsg.Content);
        }

        private async Task AcceptClientAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && isRunning)
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

                    await StartMeetingProcedure(connection, cancellationToken);

                    connection.ProcessingTask = HandleClientMessagesAsync(connection, cancellationToken)
                        .ContinueWith(async (_) =>
                    {
                        await HandleClientConnectionLossAsync(connection, cancellationToken);
                    }, cancellationToken);

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
            if (isRunning && activeConnections.Count < room!.MaximumParticipants)
            {
                _ = AcceptClientAsync(cancellationToken);
            }
        }

        private async Task HandleClientConnectionLossAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            if (room == null || connection.Participant.Id == room.HostParticipant.Id!)
            {
                return; // TODO might need to change host here
            }

            lock (activeConnections)
            {
                activeConnections.Remove(connection);
            }

            if (connection.Participant != null)
            {
                logger.Log($"Участник {connection.Participant.Name} отключился от комнаты {room?.Name}");
                if (room?.RemoveParticipantById(connection.Participant.Id) == false)
                {
                    throw new ArgumentNullException($"Не нашёлся участник с id {connection.Participant.Id}");
                }
                var chatMessage = GetParticipantLeaveMessage(connection.Participant);

                room!.AddMessage(chatMessage);
                await BroadcastMessageAsync(connection, chatMessage, cancellationToken);
            }

            await connection.DisposeAsync();

            CreateNewConnectionSlot(cancellationToken);
        }

        private async Task StartMeetingProcedure(ClientConnection connection, CancellationToken cancellationToken)
        {
            try
            {
                lock (activeConnections)
                {
                    activeConnections.Add(connection);
                }

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

                    if (requestMessage is RoomInfoRequestMessage)
                    {
                        logger.Log($"Получен RoomInfoRequest от {clientHandshake.UserId}. Отправляю RoomInfo...");
                        await SendMessageAsync(connection, GetRoomInfo(), cancellationToken);
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
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Client disconnected
                logger.Log($"Клиент {connection.Participant?.Name} отключился во время знакомства: {ex.Message}");
            }
        }

        private async Task HandleClientMessagesAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            try
            {
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

                        case ChatMessage chatMessage:
                            room!.AddMessage(chatMessage);
                            await BroadcastMessageAsync(connection, chatMessage, cancellationToken);
                            break;

                        case RoomInfoRequestMessage roomInfoRequestMessage:
                            if (!string.IsNullOrEmpty(roomInfoRequestMessage.RoomName))
                            {
                                room!.Name = roomInfoRequestMessage.RoomName;
                            }

                            if (roomInfoRequestMessage.MaxParticipants != null)
                            {
                                room!.MaximumParticipants = (int)roomInfoRequestMessage.MaxParticipants;
                            }

                            var roomInfo = GetRoomInfo();
                            await BroadcastMessageAsync(connection, roomInfo, cancellationToken);
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
                logger.Log($"Клиент {connection.Participant?.Name} отключился: {ex.Message}");
            }
        }

        private RoomInfoMessage GetRoomInfo()
            => new()
            {
                RoomId = room!.Id,
                RoomName = room.Name,
                MaxParticipants = room.MaximumParticipants,
                HostName = room.HostParticipant.Name,
                HostPCName = room.HostParticipant.PCName,
                CurrentParticipants = room.CurrentParticipants.Select(p => p.GetSerializableCopy()).ToList(),
                ChatHistory = room.ChatHistory.Select(m => m.GetSerializableCopy()).ToList(),
            };


        private static ChatMessage GetParticipantLeaveMessage(Participant participant)
            => new()
            {
                SenderName = participant.Name,
                SenderPCName = participant.PCName,
                Content = $"LEAVE:{participant.Name}:{participant.Id}",
                MessageType = MessageType.System,
                Time = TimeOnly.FromDateTime(DateTime.Now)
            };

        public async Task BroadcastMessageAsync<T>(ClientConnection sender, T message, CancellationToken ct)
            where T : class
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

        public async Task SendMessageAsync<T>(ClientConnection connection, T message, CancellationToken cancellationToken = default)
            where T : class
        {
            if (!isRunning)
            {
                throw new InvalidOperationException("Сервер не работает");
            }
            await serializer.WriteMessageAsync(connection.Pipe, message, connection.Participant.Id, cancellationToken);
        }

        public async Task StopAsync()
        {
            isRunning = false;
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
