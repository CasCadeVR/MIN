using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IMuteService"/>
public class MuteService : IMuteService
{
    private readonly IAudioCaptureService audioCaptureService;
    private readonly IVoiceDataTransmitter voiceDataTransmitter;
    private readonly IVoicePlaybackService voicePlaybackService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MuteService"/>
    /// </summary>
    public MuteService(IAudioCaptureService audioCaptureService,
        IVoiceDataTransmitter voiceDataTransmitter,
        IVoicePlaybackService voicePlaybackService)
    {
        this.audioCaptureService = audioCaptureService;
        this.voiceDataTransmitter = voiceDataTransmitter;
        this.voicePlaybackService = voicePlaybackService;
    }

    void IMuteService.MuteSelf()
    {
        audioCaptureService.Stop();
        voiceDataTransmitter.End();
    }

    void IMuteService.UnmuteSelf(Guid roomId, int subroomId)
    {
        audioCaptureService.Start();
        voiceDataTransmitter.Begin(roomId, subroomId);
    }

    void IMuteService.MuteParticipant(Guid participantId)
    {
        voicePlaybackService.RemoveParticipant(participantId);
    }

    void IMuteService.UnmuteParticipant(Guid participantId)
    {
        voicePlaybackService.AddParticipant(participantId);
    }
}
