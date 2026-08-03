using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ParticipantLeftHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    public ParticipantLeftHandler(IEventBus eventBus, ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.ParticipantLeft];

    int IMessageHandler.Priority => 3;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not ParticipantLeftMessage participantLeftMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(ParticipantLeftHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantLeftHandler)} - {message.GetType()}");
        }
        var leavingParticipantId = participantLeftMessage.Participant.Id;
        context.RoomContext.Messages.AddMessage(message);
        context.RoomContext.Participants.RemoveParticipant(leavingParticipantId);

        logger.Log($"Участник {participantLeftMessage.Participant.Name} вышел из комнаты");

        await eventBus.PublishAsync(new ParticipantLeftEvent()
        {
            RoomId = context.RoomContext.RoomId,
            Message = participantLeftMessage,
        }, context.CancellationToken);

        return HandlerResult.Success();
    }
}
