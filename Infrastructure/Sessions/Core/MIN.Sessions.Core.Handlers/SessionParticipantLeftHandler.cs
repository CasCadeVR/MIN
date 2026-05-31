using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging;
using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionParticipantLeftHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly ISessionReadyMessageResolver sessionReadyMessageResolver;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionParticipantLeftHandler"/>
    /// </summary>
    public SessionParticipantLeftHandler(IEventBus eventBus,
        ISessionReadyMessageResolver sessionReadyMessageResolver,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.sessionReadyMessageResolver = sessionReadyMessageResolver;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionParticipantLeft];

    int IMessageHandler.Priority => 11;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionParticipantLeftMessage sessionParticipantLeftMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionParticipantLeftHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionParticipantLeftHandler)} - {message.GetType()}");
        }

        var existingSessionReadyMessageId = sessionReadyMessageResolver.GetSessionReadyMessageIdOutOfSubRoomId(context.RoomContext, sessionParticipantLeftMessage.SubRoomId);

        if (existingSessionReadyMessageId == null)
        {
            return HandlerResult.Failure($"Не найдено сообщение, представляющее сессию", showErrorMessage: false);
        }

        var existing = context.RoomContext.Messages.GetMessageById(existingSessionReadyMessageId.Value) as SessionReadyMessage;
        existing!.CurrentParticipantAmount--;
        context.RoomContext.Messages.UpdateMessage(existing.Id, existing);

        await eventBus.PublishAsync(new SessionParticipantLeftEvent()
        {
            Participant = sessionParticipantLeftMessage.Participant,
            SubRoomId = sessionParticipantLeftMessage.SubRoomId,
            RoomId = context.RoomContext.RoomId,
        });

        return HandlerResult.Success();
    }
}
