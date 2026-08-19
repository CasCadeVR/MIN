using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.DI.FeatureCollection;

/// <inheritdoc cref="IVoiceFeatureCollection"/>
public class VoiceFeatureCollection : IVoiceFeatureCollection
{
    /// <inheritdoc cref="IAudioDeviceService"/>
    public IAudioDeviceService AudioDeviceService { get; }

    /// <inheritdoc cref="IMuteService"/>
    public IMuteService MuteService { get; }

    /// <inheritdoc cref="IVoicePlaybackService"/>
    public IVoicePlaybackService VoicePlayback { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VoiceFeatureCollection"/>
    /// </summary>
    public VoiceFeatureCollection(IAudioDeviceService audioDeviceService, IMuteService muteService, IVoicePlaybackService voicePlayback)
    {
        AudioDeviceService = audioDeviceService;
        MuteService = muteService;
        VoicePlayback = voicePlayback;
    }
}
