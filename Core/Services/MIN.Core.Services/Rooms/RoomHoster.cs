using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Protocol.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms;

/// <inheritdoc cref="IRoomHoster"/>
public sealed class RoomHoster : IRoomHoster
{
    private readonly IRoomFactory roomFactory;
    private readonly IRoomStore roomStore;
    private readonly IProtocolHandler protocolHandler;
    private readonly ITransport transport;
    private readonly ILoggerProvider logger;
    private readonly Dictionary<Guid, Guid> activeRooms = []; // RoomId -> ConnectionId
    private readonly Dictionary<Guid, Guid> activeConnections = []; // ConnectionId -> RoomId
    private readonly Dictionary<Guid, CancellationToken> roomCancellationTokens = [];
    private readonly HashSet<Guid> protocolPhase = [];

    /// <inheritdoc />
    public event EventHandler<RoomRawMessageReceivedEventArgs>? RawMessageReceived;

    /// <inheritdoc />
    public event EventHandler<RoomConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomHoster"/>
    /// </summary>
    public RoomHoster(IRoomFactory roomFactory,
        IRoomStore roomStore,
        IProtocolHandler protocolHandler,
        ITransport transport,
        ILoggerProvider logger)
    {
        this.roomFactory = roomFactory;
        this.roomStore = roomStore;
        this.protocolHandler = protocolHandler;
        this.transport = transport;
        this.logger = logger;

        SubscibeToEvents();
    }

    private void SubscibeToEvents()
    {
        transport.RawMessageReceived += Transport_RawMessageReceived;
        transport.ConnectionStateChanged += Transport_ConnectionStateChanged;
    }

    private async void Transport_ConnectionStateChanged(object? sender, Transport.Contracts.Events.ConnectionStateChangedEventArgs e)
    {
        if (!activeConnections.TryGetValue(e.ServerConnectionId ?? Guid.Empty, out var roomId))
        {
            return;
        }

        if (e.IsConnected)
        {
            protocolPhase.Add(e.ConnectionId);
            logger.Log($"Новое подключение к комнате {roomId}: {e.RemoteEndPoint ?? "unknown"}");

            var roomInfo = new RoomInfo(roomStore.GetRoom(roomId));
            var result = await protocolHandler.HandleServerAsync(
                e.ServerConnectionId!.Value, e.ConnectionId, roomInfo, roomCancellationTokens[roomId]);

            if (!result.IsSuccess)
            {
                logger.Log($"Клиент {e.RemoteEndPoint} не прошёл протокол: {result.ErrorMessage}");
                await transport.DisconnectClientAsync(e.ConnectionId, e.ServerConnectionId);
                return;
            }

            protocolPhase.Remove(e.ConnectionId);
            logger.Log($"Клиент {e.RemoteEndPoint} прошёл протокол для комнаты {roomId}");
        }

        var args = new RoomConnectionStateChangedEventArgs(roomId, e);
        ConnectionStateChanged?.Invoke(this, args);
    }

    private void Transport_RawMessageReceived(object? sender, Transport.Contracts.Events.RawMessageReceivedEventArgs e)
    {
        if (!activeConnections.TryGetValue(e.ServerConnectionId ?? Guid.Empty, out var roomId))
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

    async Task IRoomHoster.StartHostingAsync(RoomInfo roomInfo, bool withPortForwarding, CancellationToken cancellationToken)
    {
        if (activeRooms.ContainsKey(roomInfo.Id))
        {
            return;
        }

        roomCancellationTokens[roomInfo.Id] = cancellationToken;

        var context = roomFactory.GetOrCreateContext(roomInfo.Id);

        context.Messages.AddMessage(new SystemTextMessage()
        {
            Content = $"Комната {roomInfo.Name} была создана в {DateTime.Now.ToShortTimeString()}",
        });

        context.Messages.AddMessage(new ParticipantJoinedMessage()
        {
            Participant = new Participant(roomInfo.HostParticipant),
            RoomId = roomInfo.Id
        });

        var connectionId = await transport.StartHostingAsync(withPortForwarding, cancellationToken);

        var endpoint = transport.GetEndpoint(connectionId);
        logger.Log($"Комната создана: {endpoint} ({roomInfo.Name})");

        activeRooms[roomInfo.Id] = connectionId;
        activeConnections[connectionId] = roomInfo.Id;
    }

    Guid IRoomConnectionRelated.GetConnectionIdByRoomId(Guid roomId)
        => activeRooms.TryGetValue(roomId, out var p) ? p : throw new KeyNotFoundException();

    Guid IRoomConnectionRelated.GetRoomIdByConnectionId(Guid connectionId)
        => activeConnections.TryGetValue(connectionId, out var p) ? p : throw new KeyNotFoundException();

    async Task IRoomHoster.StopHostingAsync(Guid roomId)
    {
        if (!activeRooms.TryGetValue(roomId, out var connectionId))
        {
            return;
        }


        await transport.StopHostingAsync(connectionId);
        activeRooms.Remove(roomId);
        activeConnections.Remove(connectionId);
    }

    bool IRoomHoster.IsHosting(Guid roomId)
        => activeRooms.ContainsKey(roomId);
}
