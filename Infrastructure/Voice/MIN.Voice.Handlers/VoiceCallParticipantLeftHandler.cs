using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallParticipantLeftHandler : BaseHandler
{
    private readonly IVoiceCallStateService voiceCallStateService;
    private readonly IVoicePlaybackService voicePlaybackService;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallParticipantLeftHandler"/>
    /// </summary>
    public VoiceCallParticipantLeftHandler(IVoiceCallStateService voiceCallStateService,
        IVoicePlaybackService voicePlaybackService,
        ILoggerProvider logger) : base(logger)
    {
        this.voiceCallStateService = voiceCallStateService;
        this.voicePlaybackService = voicePlaybackService;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.VoiceParticipantLeft];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var voiceParticipantLeftMessage = (VoiceParticipantLeftMessage)message;

        var roomId = context.RoomContext.RoomId;
        var subRoomId = voiceParticipantLeftMessage.SubRoomId;
        var participant = voiceParticipantLeftMessage.Participant;

        if (voiceCallStateService.IsInVoiceCall(roomId, subRoomId))
        {
            voicePlaybackService.RemoveParticipant(participant.Id);
        }

        return HandlerResult.WithEvent(new VoiceParticipantLeftEvent()
        {
            Participant = participant,
            SubRoomId = subRoomId,
            RoomId = roomId,
        });
    }
}
