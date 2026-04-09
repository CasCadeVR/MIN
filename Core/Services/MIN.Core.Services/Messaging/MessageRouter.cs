using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Core.Services.Messaging;

/// <inheritdoc cref="IMessageRouter"/>
public sealed class MessageRouter : IMessageRouter
{
    private readonly IRoomHoster roomHoster;
    private readonly IRoomStore roomStore;
    private readonly IMessageSender messageSender;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="MessageRouter"/>
    /// </summary>
    public MessageRouter(IRoomHoster roomHoster,
        IRoomStore roomStore,
        IMessageSender messageSender,
        IParticipantConnectionRegistry participantConnectionRegistry)
    {
        this.roomHoster = roomHoster;
        this.roomStore = roomStore;
        this.messageSender = messageSender;
        this.participantConnectionRegistry = participantConnectionRegistry;
    }

    async Task IMessageRouter.RouteAsync(IMessage message, Guid roomId, Guid senderId, Guid? recipientId, CancellationToken cancellationToken)
    {
        if (roomHoster.IsHosting(roomId))
        {
            if (message.IsPublic)
            {
                await messageSender.BroadcastAsync(message, roomId, [senderId], cancellationToken);
            }
            else
            {
                if (!recipientId.HasValue)
                {
                    throw new InvalidOperationException("RecipientConnectionId required for private message");
                }

                var participantId = participantConnectionRegistry.GetConnectionIdFromParticipantId((Guid)recipientId);
                await messageSender.SendAsync(message, roomId, senderId, participantId, cancellationToken);
            }
        }
        else
        {
            var hostConnectionId = GetHostConnectionId(roomId);
            await messageSender.SendAsync(message, roomId, senderId, hostConnectionId, cancellationToken);
        }
    }

    private Guid GetHostConnectionId(Guid roomId)
    {
        var hostId = roomStore.GetRoom(roomId).HostParticipant.Id;
        if (!participantConnectionRegistry.TryGetConnectionIdFromParticipantId(hostId, out var connectionId))
        {
            throw new InvalidOperationException($"Host participant {hostId} is not registered in room {roomId}");
        }
        return connectionId;
    }
}
