using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallStartedHandler : BaseHandler
{
    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallStartedHandler"/>
    /// </summary>
    public VoiceCallStartedHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.VoiceCallStarted];

    protected override Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var voiceCallStartedMessage = (VoiceCallStartedMessage)message;
        context.RoomContext.Messages.AddMessage(voiceCallStartedMessage);

        return Task.FromResult(HandlerResult.WithEvent(new VoiceCallStartedEvent()
        {
            Message = voiceCallStartedMessage,
            RoomId = context.RoomContext.RoomId,
            Participant = context.RoomContext.Participants.GetParticipantById(voiceCallStartedMessage.Sender.Id)
        }));
    }
}
