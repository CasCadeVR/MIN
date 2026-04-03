using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms
{
    /// <summary>
    /// <inheritdoc cref="IRoomConnector"/>
    /// </summary>
    public sealed class RoomConnector : IRoomConnector
    {
        private readonly ITransport transport;
        private readonly IMessageSender messageSender;
        private readonly IMessageEncryptor encryptor;
        private readonly ILoggerProvider logger;
        private readonly HashSet<Guid> activeConnections = [];

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomConnector"/>
        /// </summary>
        public RoomConnector(ITransport transport,
            IMessageSender messageSender,
            IMessageEncryptor encryptor,
            ILoggerProvider logger)
        {
            this.transport = transport;
            this.messageSender = messageSender;
            this.encryptor = encryptor;
            this.logger = logger;
        }

        async Task<Guid> IRoomConnector.ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs, CancellationToken cancellationToken)
        {
            var connectionId = await transport.ConnectAsync(roomId, endpoint, timeoutMs, cancellationToken);
            logger.Log($"Подключились к комнате с id {roomId}, соединение с id {connectionId}");

            if (transport.IsServerHostingConnectionId(roomId, connectionId))
            {
                var selfHandshake = await encryptor.CreateSelfHandshakeMessageAsync();
                await messageSender.SendAsync(selfHandshake, roomId, selfHandshake.Participant.Id, connectionId, cancellationToken);
            }
            else
            {
                var handshakeMessage = await encryptor.CreateSelfHandshakeMessageAsync();
                await messageSender.SendAsync(handshakeMessage, roomId, handshakeMessage.Participant.Id, connectionId, cancellationToken);
            }

            activeConnections.Add(connectionId);
            return connectionId;
        }

        async Task IRoomConnector.DisconnectAsync(Guid roomId, Guid connectionId)
        {
            if (!activeConnections.Contains(connectionId))
            {
                return;
            }

            await transport.DisconnectAsync(roomId, connectionId);
            activeConnections.Remove(connectionId);
            logger.Log($"Отключились от комнаты с id {roomId}, соединение было с id {connectionId}");
        }

        bool IRoomConnector.IsConnected(Guid roomId) => activeConnections.Where(x => x == roomId).Count() != 0;
    }
}
