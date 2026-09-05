using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallParticipantJoinedHandler : BaseHandler
{
    private readonly IVoiceCallStateService voiceCallStateService;
    private readonly IVoicePlaybackService voicePlaybackService;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallParticipantJoinedHandler"/>
    /// </summary>
    public VoiceCallParticipantJoinedHandler(IVoiceCallStateService voiceCallStateService,
        IVoicePlaybackService voicePlaybackService,
        ILoggerProvider logger) : base(logger)
    {
        this.voiceCallStateService = voiceCallStateService;
        this.voicePlaybackService = voicePlaybackService;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.VoiceParticipantJoined];

    protected override Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var voiceParticipantJoinedMessage = (VoiceParticipantJoinedMessage)message;

        var roomId = context.RoomContext.RoomId;
        var subRoomId = voiceParticipantJoinedMessage.SubRoomId;
        var participant = voiceParticipantJoinedMessage.Participant;

        if (voiceCallStateService.IsInVoiceCall(roomId, subRoomId) && context.SelfId != participant.Id)
        {
            voicePlaybackService.AddParticipant(participant.Id);
        }

        return Task.FromResult(HandlerResult.WithEvent(new VoiceParticipantJoinedEvent()
        {
            Participant = context.RoomContext.Participants.GetParticipantById(participant.Id),
            SubRoomId = subRoomId,
            RoomId = roomId,
        }));
    }
}
