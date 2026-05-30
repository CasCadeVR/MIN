using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionParticipantJoinedHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionParticipantJoinedHandler"/>
    /// </summary>
    public SessionParticipantJoinedHandler(IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionParticipantJoined];

    int IMessageHandler.Priority => 11;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionParticipantJoinedMessage sessionParticipantJoinedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionParticipantJoinedHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionParticipantJoinedHandler)} - {message.GetType()}");
        }

        await eventBus.PublishAsync(new SessionParticipantJoinedEvent()
        {
            Participant = sessionParticipantJoinedMessage.Participant,
            SubRoomId = sessionParticipantJoinedMessage.SubRoomId,
            RoomId = context.RoomContext.RoomId,
        });

        return HandlerResult.Success();
    }
}
