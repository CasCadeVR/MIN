using System.Collections.Concurrent;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Messaging.Stateless.RoomRelated.Disconnect;
using MIN.Core.Services.Contracts.Interfaces.Lifecycle;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Moderation;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Transport.Contracts.Enum;

namespace MIN.Core.Services.Moderation;

/// <inheritdoc cref="INetworkErrorHandler"/>
public class NetworkErrorHandler : INetworkErrorHandler
{
    private readonly IRoomLifecycleManager lifecycleManager;
    private readonly IMessageRouter messageRouter;
    private readonly IEventBus eventBus;
    private readonly IIdentityService identityService;

    private readonly ConcurrentDictionary<Guid, Timer> rejectAckTimers = new(); // participantId / timer

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NetworkErrorHandler"/>
    /// </summary>
    public NetworkErrorHandler(IRoomLifecycleManager lifecycleManager,
        IMessageRouter messageRouter,
        IEventBus eventBus,
        IIdentityService identityService)
    {
        this.lifecycleManager = lifecycleManager;
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

        var timer = new Timer(OnRejectAckTimeout,
            new ParticipantContext(roomId, recipientId),
            Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        if (rejectAckTimers.TryRemove(recipientId, out var previousTimer))
        {
            previousTimer.Dispose();
        }

        rejectAckTimers[recipientId] = timer;

        timer.Change(TimeSpan.FromMilliseconds(timeoutMs), Timeout.InfiniteTimeSpan);
    }

    private async Task OnDisconnectAckReceived(DisconnectAckReceived e, CancellationToken cancellationToken)
    {
        if (ResetRejectAckTimer(e.ParticipantId))
        {
            await DisconnectClient(e.ParticipantId, e.RoomId);
        }
    }

    private async void OnRejectAckTimeout(object? state)
    {
        if (state is ParticipantContext connection && ResetRejectAckTimer(connection.ParticipantId))
        {
            await DisconnectClient(connection.ParticipantId, connection.RoomId);
        }
    }

    private async Task DisconnectClient(Guid participantId, Guid roomId)
        => await lifecycleManager.KickClientAsync(roomId, participantId, DisconnectReason.Kick);

    private bool ResetRejectAckTimer(Guid participantId)
    {
        if (rejectAckTimers.TryRemove(participantId, out var existingTimer))
        {
            existingTimer.Dispose();
            return true;
        }
        return false;
    }
}
