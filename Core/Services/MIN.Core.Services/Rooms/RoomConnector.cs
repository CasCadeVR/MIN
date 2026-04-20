using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms;

/// <summary>
/// <inheritdoc cref="IRoomConnector"/>
/// </summary>
public sealed class RoomConnector : IRoomConnector
{
    private readonly ITransport transport;
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;
    private readonly IMessageEncryptor encryptor;
    private readonly IRoomFactory roomFactory;
    private readonly ILoggerProvider logger;
    private readonly HashSet<Guid> activeConnections = [];

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomConnector"/>
    /// </summary>
    public RoomConnector(ITransport transport,
        IMessageRouter messageRouter,
        IIdentityService identityService,
        IMessageEncryptor encryptor,
        IRoomFactory roomFactory,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.messageRouter = messageRouter;
        this.identityService = identityService;
        this.encryptor = encryptor;
        this.roomFactory = roomFactory;
        this.logger = logger;
    }

    async Task<Guid> IRoomConnector.ConnectAsync(RoomInfo room, IEndpoint endpoint, int timeoutMs, CancellationToken cancellationToken)
    {
        try
        {
            var roomId = room.Id;
            var connectionId = await transport.ConnectAsync(roomId, endpoint, timeoutMs, cancellationToken);
            roomFactory.GetOrCreateContext(roomId).Connections.Register(connectionId, room.HostParticipant);
            logger.Log($"Подключились к комнате с {room.Name}, соединение с id {connectionId}");

            var selfHandshake = new HandshakeMessage()
            {
                Participant = identityService.SelfPartcipant.ToParticipantInfo(),
                PublicKey = await encryptor.GetLocalPublicKey(),
            };

            await messageRouter.RouteAsync(selfHandshake, roomId, selfHandshake.Participant.Id, cancellationToken);
            activeConnections.Add(roomId);
            return connectionId;
        }
        catch (TimeoutException) { return Guid.Empty; }
        catch (OperationCanceledException) { return Guid.Empty; }
    }

    async Task IRoomConnector.DisconnectAsync(Guid roomId, Guid connectionId)
    {
        if (!activeConnections.Contains(roomId))
        {
            return;
        }

        await transport.DisconnectAsync(roomId, connectionId);
        activeConnections.Remove(roomId);
        logger.Log($"Отключились от комнаты с id {roomId}, соединение было с id {connectionId}");
    }

    bool IRoomConnector.IsConnected(Guid roomId) => activeConnections.Any(x => x == roomId);
}
