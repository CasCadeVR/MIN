using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ParticipantLeftHandler : BaseHandler
{
    public ParticipantLeftHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes
        => [MessageTypeTag.ParticipantLeft];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        await Task.CompletedTask;

        var participantLeftMessage = (ParticipantLeftMessage)message;

        var leavingParticipantId = participantLeftMessage.Participant.Id;
        context.RoomContext.Messages.AddMessage(message);
        context.RoomContext.Participants.RemoveParticipant(leavingParticipantId);

        LogInfo($"Участник {participantLeftMessage.Participant.Name} ({participantLeftMessage.Participant.Id}) вышел из комнаты");

        return HandlerResult.WithEvent(new ParticipantLeftEvent()
        {
            RoomId = context.RoomContext.RoomId,
            Message = participantLeftMessage,
        });
    }
}
