using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms;

/// <inheritdoc cref="IRoomHoster"/>
public sealed class RoomHoster : IRoomHoster
{
    private readonly IRoomFactory roomFactory;
    private readonly ITransport transport;
    private readonly Dictionary<Guid, Guid> activeRooms = []; // RoomId -> ConnectionId
    private readonly Dictionary<Guid, Guid> activeConnections = []; // ConnectionId -> RoomId

    /// <inheritdoc />
    public event EventHandler<RoomRawMessageReceivedEventArgs>? RawMessageReceived;

    /// <inheritdoc />
    public event EventHandler<RoomConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomHoster"/>
    /// </summary>
    public RoomHoster(IRoomFactory roomFactory, ITransport transport)
    {
        this.roomFactory = roomFactory;
        this.transport = transport;

        SubscibeToEvents();
    }

    private void SubscibeToEvents()
    {
        transport.RawMessageReceived += Transport_RawMessageReceived;
        transport.ConnectionStateChanged += Transport_ConnectionStateChanged;
    }

    private void Transport_ConnectionStateChanged(object? sender, Transport.Contracts.Events.ConnectionStateChangedEventArgs e)
    {
        if (!activeConnections.TryGetValue(e.ServerConnectionId ?? Guid.Empty, out var roomId))
        {
            return;
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

        var args = new RoomRawMessageReceivedEventArgs(roomId, e);
        RawMessageReceived?.Invoke(this, args);
    }

    async Task IRoomHoster.StartHostingAsync(RoomInfo roomInfo, CancellationToken cancellationToken)
    {
        if (activeRooms.ContainsKey(roomInfo.Id))
        {
            return;
        }

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

        var connectionId = await transport.StartHostingAsync(cancellationToken);
        activeRooms[roomInfo.Id] = connectionId;
        activeConnections[connectionId] = roomInfo.Id;
    }

    Guid IRoomHoster.GetConnectionIdByRoomId(Guid roomId)
        => activeRooms.TryGetValue(roomId, out var p) ? p : throw new KeyNotFoundException();

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
