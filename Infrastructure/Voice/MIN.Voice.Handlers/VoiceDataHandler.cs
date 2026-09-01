using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Handlers;

internal sealed class VoiceDataHandler : BaseHandler
{
    private readonly IVoiceCallStateService voiceCallStateService;
    private readonly IVoicePlaybackService voicePlaybackService;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceDataHandler"/>
    /// </summary>
    public VoiceDataHandler(IVoiceCallStateService voiceCallStateService,
        IVoicePlaybackService voicePlaybackService,
        ILoggerProvider logger) : base(logger)
    {
        this.voiceCallStateService = voiceCallStateService;
        this.voicePlaybackService = voicePlaybackService;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.VoiceData];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var voiceData = (VoiceDataMessage)message;

        if (!voiceCallStateService.IsInVoiceCall(context.RoomContext.RoomId, voiceData.SubRoomId) && context.Role == Role.Host)
        {
            return HandlerResult.Success();
        }

        if (context.SelfId != message.SenderId)
        {
            voicePlaybackService.PlaySamples(message.SenderId, voiceData.SequenceNumber, voiceData.Data);
        }

        await Task.CompletedTask;

        return HandlerResult.WithEvent(new VoiceDataReceivedEvent()
        {
            RoomId = context.RoomContext.RoomId,
            ParticipantId = message.SenderId
        });
    }
}
