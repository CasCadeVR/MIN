using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
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
        private readonly IMessageRouter messageRouter;
        private readonly IIdentityService identityService;
        private readonly IMessageEncryptor encryptor;
        private readonly IParticipantConnectionRegistry participantConnectionRegistry;
        private readonly ILoggerProvider logger;
        private readonly HashSet<Guid> activeConnections = [];

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomConnector"/>
        /// </summary>
        public RoomConnector(ITransport transport,
            IMessageRouter messageRouter,
            IIdentityService identityService,
            IMessageEncryptor encryptor,
            IParticipantConnectionRegistry participantConnectionRegistry,
            ILoggerProvider logger)
        {
            this.transport = transport;
            this.messageRouter = messageRouter;
            this.identityService = identityService;
            this.encryptor = encryptor;
            this.participantConnectionRegistry = participantConnectionRegistry;
            this.logger = logger;
        }

        async Task<Guid> IRoomConnector.ConnectAsync(RoomInfo room, IEndpoint endpoint, int timeoutMs, CancellationToken cancellationToken)
        {
            var roomId = room.Id;
            var connectionId = await transport.ConnectAsync(roomId, endpoint, timeoutMs, cancellationToken);
            participantConnectionRegistry.Register(connectionId, room.HostParticipant);
            logger.Log($"Подключились к комнате с {room.Name}, соединение с id {connectionId}");

            var selfHandshake = new HandshakeMessage()
            {
                Participant = new ParticipantInfo(identityService.SelfPartcipant),
                PublicKey = await encryptor.GetLocalPublicKey(),
            };

            await messageRouter.RouteAsync(selfHandshake, roomId, selfHandshake.Participant.Id, Recipient.FromConnection(connectionId), cancellationToken);

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
