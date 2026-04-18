using MIN.Core.Events.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Models;
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

    async Task IMessageRouter.RouteAsync(IMessage message, Guid roomId, Guid senderId, Recipient recipient, CancellationToken cancellationToken)
    {
        if (roomHoster.IsHosting(roomId))
        {
            if (message.IsPublic)
            {
                await eventBus.PublishAsync(new LocalMessageRecievedEvent(message, roomId, senderId), cancellationToken);
            }
            else
            {
                if (!recipient.IsLocal)
                {
                    var recipientConnectionId = recipient.ResolveAsync(participantConnectionRegistry);
                    await messageSender.SendAsync(message, roomId, senderId, recipientConnectionId, cancellationToken);
                }
                else
                {
                    await eventBus.PublishAsync(new LocalMessageRecievedEvent(message, roomId, senderId), cancellationToken);
                }
            }
        }
        else
        {
            if (message.IsPublic)
            {
                await eventBus.PublishAsync(new LocalMessageRecievedEvent(message, roomId, senderId), cancellationToken);
            }

            var hostConnectionId = GetHostConnectionId(roomId);
            await messageSender.SendAsync(message, roomId, senderId, hostConnectionId, cancellationToken);
        }
    }

    private Guid GetHostConnectionId(Guid roomId)
    {
        var hostId = roomStore.GetRoomHostParticipantId(roomId);
        if (!participantConnectionRegistry.TryGetConnectionIdFromParticipantId(roomId, hostId, out var connectionId))
        {
            throw new InvalidOperationException($"Host participant {hostId} is not registered in room {roomId}");
        }
        return connectionId;
    }
}
