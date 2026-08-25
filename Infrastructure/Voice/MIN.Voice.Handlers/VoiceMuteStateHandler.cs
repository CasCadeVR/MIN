using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceMuteStateHandler : BaseHandler
{
    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceMuteStateHandler"/>
    /// </summary>
    public VoiceMuteStateHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.VoiceMuteState];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var voiceMuteStateChangedMessage = (VoiceMuteStateChangedMessage)message;

        return HandlerResult.WithEvent(new VoiceMuteStateChangedEvent()
        {
            RoomId = context.RoomContext.RoomId,
            Muted = voiceMuteStateChangedMessage.IsMuted,
            ParticipantId = voiceMuteStateChangedMessage.SenderId,
        });
    }
}
