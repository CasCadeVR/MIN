using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IMuteService"/>
public class MuteService : IMuteService
{
    private readonly IAudioCaptureService audioCaptureService;
    private readonly IVoiceDataTransmitter voiceDataTransmitter;
    private readonly IVoicePlaybackService voicePlaybackService;
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MuteService"/>
    /// </summary>
    public MuteService(IAudioCaptureService audioCaptureService,
        IVoiceDataTransmitter voiceDataTransmitter,
        IVoicePlaybackService voicePlaybackService,
        IMessageRouter messageRouter,
        IIdentityService identityService)
    {
        this.audioCaptureService = audioCaptureService;
        this.voiceDataTransmitter = voiceDataTransmitter;
        this.voicePlaybackService = voicePlaybackService;
        this.messageRouter = messageRouter;
        this.identityService = identityService;
    }

    async Task IMuteService.MuteSelf(Guid roomId, int subroomId, CancellationToken cancellationToken)
    {
        await SendMutedState(roomId, subroomId, muted: true, cancellationToken);
        audioCaptureService.Stop();
        voiceDataTransmitter.End();
    }

    async Task IMuteService.UnmuteSelf(Guid roomId, int subroomId, CancellationToken cancellationToken)
    {
        await SendMutedState(roomId, subroomId, muted: false, cancellationToken);
        audioCaptureService.Start();
        voiceDataTransmitter.Begin(roomId, subroomId);
    }

    private async Task SendMutedState(Guid roomId, int subroomId, bool muted, CancellationToken cancellationToken)
        => await messageRouter.RouteAsync(new VoiceMuteStateChangedMessage()
        {
            SubRoomId = subroomId,
            IsMuted = muted
        }, roomId, identityService.SelfParticipant.Id, cancellationToken);

    void IMuteService.MuteParticipant(Guid participantId)
    {
        voicePlaybackService.RemoveParticipant(participantId);
    }

    void IMuteService.UnmuteParticipant(Guid participantId)
    {
        voicePlaybackService.AddParticipant(participantId);
    }
}
