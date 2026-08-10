using MIN.Voice.Services.Contacts.Models;

namespace MIN.Voice.Services.Contacts.Interfaces;

/// <summary>
/// Помошник в выборе звуковое устройство ввода/вывода
/// </summary>
public interface IAudioDeviceService
{
    /// <summary>
    /// Получить микрофоны
    /// </summary>
    IReadOnlyList<AudioDeviceInfo> GetInputDevices();

    /// <summary>
    /// Получить динамики
    /// </summary>
    IReadOnlyList<AudioDeviceInfo> GetOutputDevices();
}
