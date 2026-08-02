using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Messaging.Stateless.RoomRelated.Ping;
using MIN.Core.Protocol.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Services.Rooms;

/// <summary>
/// <inheritdoc cref="IRoomConnector"/>
/// </summary>
public sealed class RoomConnector : IRoomConnector
{
    private readonly ITransport transport;
    private readonly IProtocolHandler protocolHandler;
    private readonly IRoomStore roomStore;
    private readonly IRoomFactory roomFactory;
    private readonly IMessageSender messageSender;
    private readonly IEventBus eventBus;
    private readonly IIdentityService identityService;
    private readonly IMessageEncryptor encryptor;
    private readonly IPingService pingService;
    private readonly IVersionProvider versionProvider;
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
        IProtocolHandler protocolHandler,
        IRoomStore roomStore,
        IRoomFactory roomFactory,
        IMessageSender messageSender,
        IEventBus eventBus,
        IIdentityService identityService,
        IPingService pingService,
        IMessageEncryptor encryptor,
        IVersionProvider versionProvider,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.protocolHandler = protocolHandler;
        this.roomStore = roomStore;
        this.roomFactory = roomFactory;
        this.messageSender = messageSender;
        this.eventBus = eventBus;
        this.identityService = identityService;
        this.pingService = pingService;
        this.encryptor = encryptor;
        this.versionProvider = versionProvider;
        this.logger = logger;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        transport.RawMessageReceived += Transport_RawMessageReceived;
        transport.ConnectionStateChanged += Transport_ConnectionStateChanged;
    }

    private async void Transport_ConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        if (!activeConnections.TryGetValue(e.ConnectionId, out var roomId))
        {
            return;
        }

        if (!e.IsConnected)
        {
            // Here we are cleaning for this service specifically
            pingService.OnConnectionTimeout -= PingService_OnConnectionTimeout;
            await pingService.UnregisterHeartbeatSession(Role.Client, roomId, e.ConnectionId);
            activeRooms.Remove(roomId);
            activeConnections.Remove(e.ConnectionId);

            logger.Log($"Отключились от комнаты с id {roomId}, соединение было с id {e.ConnectionId}");
        }

        var args = new RoomConnectionStateChangedEventArgs(roomId, e);
        // ConnectionMonitor will clean the rest of a room
        ConnectionStateChanged?.Invoke(this, args);
    }

    private void Transport_RawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
    {
        if (!activeConnections.TryGetValue(e.ConnectionId, out var roomId))
        {
            return;
        }

        var args = new RoomRawMessageReceivedEventArgs(roomId, e);
        RawMessageReceived?.Invoke(this, args);
    }

    async Task<ConnectionResult> IRoomConnector.ConnectAsync(IEndpoint endpoint, CancellationToken cancellationToken)
    {
        var connectionResult = new ConnectionResult();

        try
        {
            logger.Log($"Подключаюсь к {endpoint}");

            connectionResult.ConnectionId = await transport.ConnectAsync(endpoint, cancellationToken);

            var result = await protocolHandler.HandleClientAsync(connectionResult.ConnectionId, cancellationToken);

            if (!result.IsSuccess)
            {
                logger.Log($"Протокол не пройден для {endpoint}: {result.ErrorMessage}", LogLevel.Error);
                await transport.DisconnectAsync(connectionResult.ConnectionId, DisconnectReason.ProtocolError);
                throw new InvalidOperationException(result.ErrorMessage);
            }

            if (roomStore.RoomExists(result.RoomInfo.Id))
            {
                throw new InvalidOperationException("Вы уже подключены к этой комнате");
            }

            connectionResult.RoomId = result.RoomInfo.Id;
            logger.Log($"Протокол успешен, комната {connectionResult.RoomId}");

            await pingService.RegisterHeartbeatSession(Role.Client, connectionResult.RoomId, connectionResult.ConnectionId);
            pingService.OnConnectionTimeout += PingService_OnConnectionTimeout;
            pingService.OnPingRequested += PingService_OnPingRequested;

            var selfParticipant = identityService.SelfParticipant.ToParticipantInfo();

            roomFactory.GetOrCreateContext(connectionResult.RoomId)
                .Connections.RegisterLocalParticipant(selfParticipant);

            roomStore.Register(new Room(result.RoomInfo));

            logger.Log($"Подключились к комнате с id {connectionResult.RoomId}, соединение с id {connectionResult.ConnectionId}");

            var selfHandshake = new HandshakeMessage()
            {
                Participant = selfParticipant,
                PublicKey = await encryptor.GetLocalPublicKey(),
                Version = versionProvider.Version
            };

            await messageSender.SendAsync(selfHandshake, connectionResult.RoomId, connectionResult.ConnectionId, cancellationToken);
            activeRooms[connectionResult.RoomId] = connectionResult.ConnectionId;
            activeConnections[connectionResult.ConnectionId] = connectionResult.RoomId;
            return connectionResult;
        }
        catch (TimeoutException) { return connectionResult; }
        catch (OperationCanceledException) { return connectionResult; }
        catch
        {
            if (connectionResult.RoomId != Guid.Empty)
            {
                roomStore.Remove(connectionResult.RoomId);
                roomFactory.DestroyContext(connectionResult.RoomId);
            }

            throw;
        }
    }

    private async Task PingService_OnPingRequested(Guid roomId, Guid connectionId) => await messageSender.SendAsync(new PingMessage(), roomId, connectionId);
    private async Task PingService_OnConnectionTimeout(Guid roomId, Guid connectionId) => await DisconnectAsync(roomId, connectionId, DisconnectReason.Timeout);

    /// <inheritdoc />
    public async Task DisconnectAsync(Guid roomId, Guid connectionId, DisconnectReason reason)
    {
        if (!activeRooms.ContainsKey(roomId))
        {
            return;
        }

        logger.Log($"Я сам иницирую отключение от комнаты с id {roomId} с соединением {connectionId}: {reason}");

        // Transport will fire event, where it would cleanup further
        await transport.DisconnectAsync(connectionId, reason);
    }

    Guid IRoomConnectionRelated.GetConnectionIdByRoomId(Guid roomId)
         => activeRooms.TryGetValue(roomId, out var p) ? p : throw new KeyNotFoundException();

    Guid IRoomConnectionRelated.GetRoomIdByConnectionId(Guid connectionId)
        => activeConnections.TryGetValue(connectionId, out var p) ? p : throw new KeyNotFoundException();

    bool IRoomConnector.IsConnected(Guid roomId) => activeRooms.ContainsKey(roomId);
}
