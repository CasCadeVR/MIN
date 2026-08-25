using MIN.Chat.Events;
using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Handlers;

internal sealed class OnlineStatusHandler : BaseHandler
{
    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatTextHandler"/>
    /// </summary>
    public OnlineStatusHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.OnlineStatusChanged];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var onlineStatusChangedMessage = (OnlineStatusChangedMessage)message;

        if (context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var participant))
        {
            var status = onlineStatusChangedMessage.Status;
            participant!.CurrentStatus = status;
            if (status == OnlineStatus.Offline)
            {
                participant.LastSeenOnline = DateTime.Now;
            }
        }

        return HandlerResult.WithEvent(new OnlineStatusChangedEvent()
        {
            RoomId = context.RoomContext.RoomId,
            Status = onlineStatusChangedMessage.Status,
            SenderId = message.SenderId,
        });
    }
}
