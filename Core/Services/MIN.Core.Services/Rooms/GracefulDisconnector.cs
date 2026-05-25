using System.Collections.Concurrent;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.Stateless.RoomRelated.Disconnect;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms;

/// <inheritdoc cref="IGracefulDisconnector"/>
public class GracefulDisconnector : IGracefulDisconnector
{
    private readonly ITransport transport;
    private readonly IRoomConnectionResolver roomConnectionResolver;
    private readonly IMessageSender messageSender;
    private readonly IEventBus eventBus;
    private readonly ConcurrentDictionary<Guid, Timer> rejectAckTimers = new();

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GracefulDisconnector"/>
    /// </summary>
    public GracefulDisconnector(ITransport transport,
        IRoomConnectionResolver roomConnectionResolver,
        IMessageSender messageSender,
        IEventBus eventBus)
    {
        this.eventBus = eventBus;
        this.roomConnectionResolver = roomConnectionResolver;
        this.messageSender = messageSender;
        this.transport = transport;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        eventBus.Subscribe<DisconnectAckReceived>(OnDisconnectAckReceived);
    }

    private async Task OnDisconnectAckReceived(DisconnectAckReceived e, CancellationToken cancellationToken)
    {
        ResetRejectAckTimer(e.ConnectionId);
        await DisconnectClient(e.ConnectionId, e.RoomId);
    }

    async Task IGracefulDisconnector.DisconnectWithReasonAsync(Guid connectionId, Guid roomId, string reason, int timeoutMs)
    {
        var disconnectMessage = new DisconnectMessage()
        {
            Reason = reason
        };
        await messageSender.SendAsync(disconnectMessage, roomId, connectionId, CancellationToken.None);
        var timer = new Timer(
            OnRejectAckTimeout,
            new ConnectionResult(roomId, connectionId),
            DateTime.UtcNow.AddMilliseconds(timeoutMs) - DateTime.UtcNow,
            Timeout.InfiniteTimeSpan);

        rejectAckTimers.TryAdd(connectionId, timer);
    }

    private async void OnRejectAckTimeout(object? state)
    {
        if (state is ConnectionResult connection)
        {
            await DisconnectClient(connection.ConnectionId, connection.RoomId);
            ResetRejectAckTimer(connection.ConnectionId);
        }
    }

    private async Task DisconnectClient(Guid connectionId, Guid roomId)
    {
        var serverConnectionId = roomConnectionResolver.GetServerConnectionIdByRoomId(connectionId, roomId);
        await transport.DisconnectClientAsync(connectionId, serverConnectionId);
    }

    private void ResetRejectAckTimer(Guid connectionId)
    {
        if (rejectAckTimers.TryGetValue(connectionId, out var existingTimer))
        {
            existingTimer.Dispose();
        }
    }
}
