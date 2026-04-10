using MIN.Core.Events.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Interfaces.Stores;
using MIN.Core.Services.Contracts.Models;

namespace MIN.Core.Services.Messaging;

/// <inheritdoc cref="IMessageRouter"/>
public sealed class MessageRouter : IMessageRouter
{
    private readonly IRoomHoster roomHoster;
    private readonly IRoomStore roomStore;
    private readonly IParticipantStore participantStore;
    private readonly IEventBus eventBus;
    private readonly IMessageSender messageSender;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="MessageRouter"/>
    /// </summary>
    public MessageRouter(IRoomHoster roomHoster,
        IRoomStore roomStore,
        IParticipantStore participantStore,
        IEventBus eventBus,
        IMessageSender messageSender,
        IParticipantConnectionRegistry participantConnectionRegistry)
    {
        this.roomHoster = roomHoster;
        this.roomStore = roomStore;
        this.participantStore = participantStore;
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
        if (!participantConnectionRegistry.TryGetConnectionIdFromParticipantId(hostId, out var connectionId))
        {
            throw new InvalidOperationException($"Host participant {hostId} is not registered in room {roomId}");
        }
        return connectionId;
    }
}
