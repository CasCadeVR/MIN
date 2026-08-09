using MIN.Voice.Services.Contacts.Models;

namespace MIN.Voice.Services.Contacts.Interfaces
{
    /// <summary>
    /// Сервис захвата звука
    /// </summary>
    public interface IAudioCaptureService : IDisposable
    {
        /// <summary>
        /// Начать захватывать звук микрофона
        /// </summary>
        void Start();

        /// <summary>
        /// Остановить захват звука микрофона
        /// </summary>
        void Stop();

        /// <summary>
        /// Получен frame звука для отправки
        /// </summary>
        event EventHandler<AudioFrame>? FrameCaptured; // 640 байт PCM
    }
}
