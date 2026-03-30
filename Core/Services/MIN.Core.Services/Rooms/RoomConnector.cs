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
        private readonly HashSet<Guid> activeConnections = new();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomConnector"/>
        /// </summary>
        public RoomConnector(ITransport transport, IMessageSender messageSender, IMessageEncryptor encryptor, ILoggerProvider logger)
        {
            this.transport = transport;
            this.messageSender = messageSender;
            this.encryptor = encryptor;
            this.logger = logger;
        }

        public async Task<Guid> ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs, CancellationToken cancellationToken = default)
        {
            var connectionId = await transport.ConnectAsync(roomId, endpoint, timeoutMs, cancellationToken);
            logger.Log($"Подключились к комнате с id {roomId}, соединение с id {connectionId}");

            var handshakeMessage = await encryptor.CreateSelfHandshakeMessageAsync();
            await messageSender.SendAsync(handshakeMessage, roomId, connectionId, cancellationToken);

            activeConnections.Add(connectionId);
            return connectionId;
        }

        public async Task DisconnectAsync(Guid roomId, Guid connectionId)
        {
            if (!activeConnections.Contains(connectionId))
            {
                return;
            }

            await transport.DisconnectAsync(roomId, connectionId);
            activeConnections.Remove(connectionId);
            logger.Log($"Отключились от комнаты с id {roomId}, соединение было с id {connectionId}");
        }

        public bool IsConnected(Guid roomId) => activeConnections.Any();
    }
}
