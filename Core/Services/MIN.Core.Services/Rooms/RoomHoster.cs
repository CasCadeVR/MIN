using System.Collections.Concurrent;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Protocol.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Constants;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Models;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms;

/// <inheritdoc cref="IRoomHoster"/>
public sealed class RoomHoster : IRoomHoster
{
    private readonly IRoomFactory roomFactory;
    private readonly IProtocolHandler protocolHandler;
    private readonly ITransport transport;
    private readonly IRoomStore roomStore;
    private readonly IEventBus eventBus;
    private readonly ISubRoomManager subRoomManager;
    private readonly IPingService pingService;
    private readonly IRoomConnectionRegistry registry;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, RoomInfo> readyRoomInfos = []; // RoomId -> RoomInfo
    private readonly Dictionary<Guid, CancellationTokenSource> roomCancellationTokenSources = [];
    private readonly HashSet<Guid> protocolPhase = [];

    /// <inheritdoc />
    public event EventHandler<RoomRawMessageReceivedEventArgs>? RawMessageReceived;

    /// <inheritdoc />
    public event EventHandler<RoomConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomHoster"/>
    /// </summary>
    public RoomHoster(IRoomFactory roomFactory,
        IProtocolHandler protocolHandler,
        ITransport transport,
        IRoomStore roomStore,
        IEventBus eventBus,
        ISubRoomManager subRoomManager,
        IPingService pingService,
        IRoomConnectionRegistry registry,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.roomFactory = roomFactory;
        this.protocolHandler = protocolHandler;
        this.transport = transport;
        this.roomStore = roomStore;
        this.eventBus = eventBus;
        this.subRoomManager = subRoomManager;
        this.pingService = pingService;
        this.registry = registry;
        this.identityService = identityService;
        this.logger = logger;

        SubscibeToEvents();
    }

    private void SubscibeToEvents()
    {
        transport.RawMessageReceived += Transport_RawMessageReceived;
        transport.ConnectionStateChanged += Transport_ConnectionStateChanged;
        pingService.OnConnectionTimeout += PingService_OnConnectionTimeout;
    }

    private async void Transport_ConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        if (!registry.TryGetRoomIdByServerConnectionId(e.ServerConnectionId, out var roomId))
        {
            return;
        }

        if (e.IsConnected)
        {
            protocolPhase.Add(e.ConnectionId);
            logger.Log($"Новое подключение к комнате {roomId}: {e.RemoteEndPoint ?? "unknown"}");

            var roomInfo = readyRoomInfos[roomId];
            var result = await protocolHandler.HandleServerAsync(
                e.ServerConnectionId!.Value, e.ConnectionId, roomInfo, roomCancellationTokenSources[roomId].Token);

            if (!result.IsSuccess)
            {
                logger.Log($"Клиент {e.RemoteEndPoint} не прошёл протокол: {result.ErrorMessage}");
                await transport.DisconnectClientAsync(e.ConnectionId, e.ServerConnectionId, DisconnectReason.ProtocolError);
                return;
            }

            protocolPhase.Remove(e.ConnectionId);
            logger.Log($"Клиент {e.RemoteEndPoint} прошёл протокол для комнаты {roomId}");

            await pingService.RegisterHeartbeatSession(Role.Host, roomId, e.ConnectionId);
        }
        else
        {
            await pingService.UnregisterHeartbeatSession(Role.Host, roomId, e.ConnectionId);
        }

        var args = new RoomConnectionStateChangedEventArgs(roomId, e);
        ConnectionStateChanged?.Invoke(this, args);
    }

    private async Task PingService_OnConnectionTimeout(Guid roomId, Guid connectionId)
    {
        if (!registry.TryGetServerConnectionIdByRoomId(roomId, out var serverConnectionId))
        {
            return;
        }

        await transport.DisconnectClientAsync(connectionId, serverConnectionId, DisconnectReason.Timeout);
    }

    private void Transport_RawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
    {
        if (!registry.TryGetRoomIdByServerConnectionId(e.ServerConnectionId, out var roomId))
        {
            return;
        }

        if (protocolPhase.Contains(e.ConnectionId))
        {
            return;
        }

        var args = new RoomRawMessageReceivedEventArgs(roomId, e);
        RawMessageReceived?.Invoke(this, args);
    }

    async Task<Room> IRoomHoster.StartHostingAsync(RoomInfo roomInfo, NetworkOptions networkOptions, CancellationToken cancellationToken)
    {
        if (registry.GetServerConnectionCount() + 1 > ServicesConstants.MaximumRoomHosts)
        {
            throw new InvalidOperationException($"Можно хостить максимум {ServicesConstants.MaximumRoomHosts} комнат");
        }

        var roomId = roomInfo.Id;

        if (registry.IsHosting(roomId))
        {
            return roomStore.GetRoom(roomId);
        }

        roomCancellationTokenSources[roomId] = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var context = roomFactory.GetOrCreateContext(roomId);

        var localParticipant = identityService.SelfParticipant.ToParticipantInfo();

        context.Connections.RegisterLocalParticipant(localParticipant);
        roomInfo.HostParticipant = localParticipant;

        var room = new Room(roomInfo);

        roomStore.Register(room);

        context.Messages.AddMessage(new SystemTextMessage()
        {
            Content = $"Комната {roomInfo.Name} была создана в {DateTime.Now.ToShortTimeString()}",
        });

        context.Messages.AddMessage(new ParticipantJoinedMessage()
        {
            Participant = new Participant(localParticipant)
        });

        room.TotalMessageCount = context.Messages.GetMessageCount();

        var connectionId = await transport.StartHostingAsync(cancellationToken);
        room.ConnectionAddresses = await transport.SetUpAndGetEndpoints(connectionId, networkOptions, cancellationToken: cancellationToken);
        room.LocalRoomSettings.NetworkOptions = networkOptions;

        context.Participants.AddParticipant(new Participant(localParticipant));

        logger.Log($"Комната создана: {string.Join(',', room.ConnectionAddresses)} ({roomInfo.Name})");

        registry.RegisterServerConnection(roomId, connectionId);
        readyRoomInfos[roomId] = roomInfo;

        return roomStore.GetRoom(roomId);
    }

    async Task<IEnumerable<IEndpoint>> IRoomHoster.UpdateNetworkOptions(Guid roomId, NetworkOptions newNetworkOptions, CancellationToken cancellationToken)
    {
        var room = roomStore.GetRoom(roomId);

        if (!registry.TryGetServerConnectionIdByRoomId(roomId, out var connectionId))
        {
            return room.ConnectionAddresses;
        }

        var newEndpoints = await transport.SetUpAndGetEndpoints(connectionId, newNetworkOptions, room.LocalRoomSettings.NetworkOptions, cancellationToken);
        room.ConnectionAddresses = newEndpoints;
        room.LocalRoomSettings.NetworkOptions = newNetworkOptions;

        return room.ConnectionAddresses;
    }

    async Task IRoomHoster.StopHostingAsync(Guid roomId)
    {
        if (!registry.TryGetServerConnectionIdByRoomId(roomId, out var connectionId))
        {
            return;
        }

        if (roomCancellationTokenSources.TryGetValue(roomId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            roomCancellationTokenSources.Remove(roomId);
        }

        await transport.StopHostingAsync(connectionId);
        subRoomManager.ClearRoomSubRooms(roomId);

        registry.UnregisterServerConnection(roomId);

        readyRoomInfos.TryRemove(roomId, out _);

        roomStore.Remove(roomId);
        roomFactory.DestroyContext(roomId);

        await eventBus.PublishAsync(new RoomClosedEvent() { RoomId = roomId });
    }
}
