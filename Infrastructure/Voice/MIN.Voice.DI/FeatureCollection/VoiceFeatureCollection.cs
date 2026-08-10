using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.DI.FeatureCollection;

/// <inheritdoc cref="IVoiceFeatureCollection"/>
public class VoiceFeatureCollection : IVoiceFeatureCollection
{
    /// <inheritdoc cref="IAudioDeviceService"/>
    public IAudioDeviceService AudioDeviceService { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VoiceFeatureCollection"/>
    /// </summary>
    public VoiceFeatureCollection(IAudioDeviceService audioDeviceService)
    {
        AudioDeviceService = audioDeviceService;
    }
}
