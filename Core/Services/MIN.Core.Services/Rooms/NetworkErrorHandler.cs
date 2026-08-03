using System.Collections.Concurrent;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Messaging.Stateless.RoomRelated.Disconnect;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms;

/// <inheritdoc cref="INetworkErrorHandler"/>
public class NetworkErrorHandler : INetworkErrorHandler
{
    private readonly ITransport transport;
    private readonly IRoomConnectionResolver roomConnectionResolver;
    private readonly IRoomFactory roomFactory;
    private readonly IMessageRouter messageRouter;
    private readonly IEventBus eventBus;
    private readonly IIdentityService identityService;
    private readonly ConcurrentDictionary<Guid, Timer> rejectAckTimers = new(); // participantId / timer

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NetworkErrorHandler"/>
    /// </summary>
    public NetworkErrorHandler(ITransport transport,
        IRoomConnectionResolver roomConnectionResolver,
        IRoomFactory roomFactory,
        IMessageRouter messageRouter,
        IEventBus eventBus,
        IIdentityService identityService)
    {
        this.transport = transport;
        this.roomConnectionResolver = roomConnectionResolver;
        this.roomFactory = roomFactory;
        this.messageRouter = messageRouter;
        this.eventBus = eventBus;
        this.identityService = identityService;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        eventBus.Subscribe<DisconnectAckReceived>(OnDisconnectAckReceived);
    }

    async Task INetworkErrorHandler.SendErrorAsync(string message, Guid recipientId, Guid roomId, bool critical, int timeoutMs)
    {
        var selfId = identityService.SelfParticipant.Id;

        if (!critical)
        {
            await messageRouter.RouteAsync(new ErrorMessage()
            {
                Message = message,
                RecipientId = recipientId,
            }, roomId, selfId, CancellationToken.None);
            return;
        }

        var disconnectMessage = new DisconnectMessage()
        {
            Reason = message,
            RecipientId = recipientId,
        };

        await messageRouter.RouteAsync(disconnectMessage, roomId, selfId,
            CancellationToken.None, broadcastExcludeIds: [identityService.SelfParticipant.Id]);

        var timer = new Timer(
            OnRejectAckTimeout,
            new ParticipantContext(roomId, recipientId),
            DateTime.UtcNow.AddMilliseconds(timeoutMs) - DateTime.UtcNow,
            Timeout.InfiniteTimeSpan);

        rejectAckTimers.TryAdd(recipientId, timer);
    }

    private async Task OnDisconnectAckReceived(DisconnectAckReceived e, CancellationToken cancellationToken)
    {
        ResetRejectAckTimer(e.ParticipantId);
        await DisconnectClient(e.ParticipantId, e.RoomId);
    }

    private async void OnRejectAckTimeout(object? state)
    {
        if (state is ParticipantContext connection)
        {
            await DisconnectClient(connection.ParticipantId, connection.RoomId);
            ResetRejectAckTimer(connection.ParticipantId);
        }
    }

    private async Task DisconnectClient(Guid participantId, Guid roomId)
    {
        roomFactory.TryGetContext(roomId, out var context);
        if (context == null)
        {
            return;
        }
        var connectionId = context.Connections.GetConnectionIdFromParticipantId(participantId);
        var serverConnectionId = roomConnectionResolver.GetServerConnectionIdByRoomId(connectionId, roomId);
        await transport.DisconnectClientAsync(connectionId, serverConnectionId, DisconnectReason.Kick);
    }

    private void ResetRejectAckTimer(Guid participantId)
    {
        if (rejectAckTimers.TryGetValue(participantId, out var existingTimer))
        {
            existingTimer.Dispose();
        }
    }
}
