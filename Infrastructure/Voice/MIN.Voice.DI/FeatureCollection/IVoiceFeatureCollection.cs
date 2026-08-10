using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.DI.FeatureCollection;

/// <summary>
/// Набор функциональностей для Voice
/// </summary>
public interface IVoiceFeatureCollection
{
    /// <inheritdoc cref="IAudioDeviceService"/>
    IAudioDeviceService AudioDeviceService { get; }
}
