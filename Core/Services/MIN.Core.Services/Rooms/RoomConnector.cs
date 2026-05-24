using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
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
    private readonly IMessageSender messageSender;
    private readonly IIdentityService identityService;
    private readonly IMessageEncryptor encryptor;
    private readonly ILoggerProvider logger;
    private readonly Dictionary<Guid, Guid> activeRooms = []; // RoomId -> ConnectionId
    private readonly Dictionary<Guid, Guid> activeConnections = []; // ConnectionId -> RoomId

    /// <inheritdoc />
    public event EventHandler<RoomRawMessageReceivedEventArgs>? RawMessageReceived;

    /// <inheritdoc />
    public event EventHandler<RoomConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomConnector"/>
    /// </summary>
    public RoomConnector(ITransport transport,
        IMessageSender messageSender,
        IIdentityService identityService,
        IMessageEncryptor encryptor,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.messageSender = messageSender;
        this.identityService = identityService;
        this.encryptor = encryptor;
        this.logger = logger;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        transport.RawMessageReceived += Transport_RawMessageReceived;
        transport.ConnectionStateChanged += Transport_ConnectionStateChanged;
        ;
    }

    private void Transport_ConnectionStateChanged(object? sender, Transport.Contracts.Events.ConnectionStateChangedEventArgs e)
    {
        if (!activeConnections.TryGetValue(e.ConnectionId, out var roomId))
        {
            return;
        }

        if (!e.IsConnected)
        {
            activeConnections.Remove(roomId);
            logger.Log($"Отключились от комнаты с id {roomId}, соединение было с id {e.ConnectionId}");
        }

        var args = new RoomConnectionStateChangedEventArgs(roomId, e);
        ConnectionStateChanged?.Invoke(this, args);
    }

    private void Transport_RawMessageReceived(object? sender, Transport.Contracts.Events.RawMessageReceivedEventArgs e)
    {
        if (!activeConnections.TryGetValue(e.ConnectionId, out var roomId))
        {
            return;
        }

        var args = new RoomRawMessageReceivedEventArgs(roomId, e);
        RawMessageReceived?.Invoke(this, args);
    }

    async Task<Guid> IRoomConnector.ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs, CancellationToken cancellationToken)
    {
        try
        {
            var connectionId = await transport.ConnectAsync(endpoint, timeoutMs, cancellationToken);
            logger.Log($"Подключились к комнате с id {roomId}, соединение с id {connectionId}");

            var selfHandshake = new HandshakeMessage()
            {
                Participant = identityService.SelfParticipant.ToParticipantInfo(),
                PublicKey = await encryptor.GetLocalPublicKey(),
            };

            await messageSender.SendAsync(selfHandshake, roomId, connectionId, cancellationToken);
            activeRooms[roomId] = connectionId;
            activeConnections[connectionId] = roomId;
            return connectionId;
        }
        catch (TimeoutException) { return Guid.Empty; }
        catch (OperationCanceledException) { return Guid.Empty; }
    }

    async Task IRoomConnector.DisconnectAsync(Guid roomId, Guid connectionId)
    {
        if (!activeRooms.ContainsKey(roomId))
        {
            return;
        }

        await transport.DisconnectAsync(connectionId);
        activeRooms.Remove(roomId);
        activeConnections.Remove(connectionId);
        logger.Log($"Отключились от комнаты с id {roomId}, соединение было с id {connectionId}");
    }

    bool IRoomConnector.IsConnected(Guid roomId) => activeRooms.ContainsKey(roomId);
}
