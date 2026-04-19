using MIN.Core.Events.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;

namespace MIN.Core.Services.Messaging;

/// <inheritdoc cref="IMessageRouter"/>
public sealed class MessageRouter : IMessageRouter
{
    private readonly IRoomHoster roomHoster;
    private readonly IRoomStore roomStore;
    private readonly IEventBus eventBus;
    private readonly IMessageSender messageSender;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="MessageRouter"/>
    /// </summary>
    public MessageRouter(IRoomHoster roomHoster,
        IRoomStore roomStore,
        IEventBus eventBus,
        IMessageSender messageSender,
        IParticipantConnectionRegistry participantConnectionRegistry)
    {
        this.roomHoster = roomHoster;
        this.roomStore = roomStore;
        this.eventBus = eventBus;
        this.messageSender = messageSender;
        this.participantConnectionRegistry = participantConnectionRegistry;
    }

    async Task IMessageRouter.RouteAsync(IMessage message, Guid roomId, Guid senderId, CancellationToken cancellationToken)
    {
        message.SenderId = senderId;

        if (roomHoster.IsHosting(roomId))
        {
            // Server

            // If its public, dispatcher will broadcast to anyone except server and sender (cuz they already handled it)
            // Regardless of recipient - they had to put recipientId and public = false if they wanted it to be private
            // So basically dispatcher will handle all of it

            await PublishLocally(message, roomId, cancellationToken);
        }
        else
        {
            // Client

            if (message.RequiresLocalDuplication)
            {
                await PublishLocally(message, roomId, cancellationToken);
            }

            var hostId = roomStore.GetRoomHostParticipantId(roomId);
            var hostConnectionId = GetHostConnectionId(roomId, hostId);
            await messageSender.SendAsync(message, roomId, hostConnectionId, cancellationToken);
        }
    }

    private async Task PublishLocally(IMessage message, Guid roomId, CancellationToken cancellationToken)
        => await eventBus.PublishAsync(new LocalMessageRecievedEvent(message, roomId), cancellationToken);

    private Guid GetHostConnectionId(Guid roomId, Guid hostId)
    {
        if (!participantConnectionRegistry.TryGetConnectionIdFromParticipantId(roomId, hostId, out var connectionId))
        {
            throw new InvalidOperationException($"Host participant {hostId} is not registered in room {roomId}");
        }
        return connectionId;
    }
}
