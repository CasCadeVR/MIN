namespace MIN.Voice.Services.Contacts.Interfaces;

/// <summary>
/// Сервис определения, содержит ли аудиокадр голосовую активность, с учётом
/// чувствительности, адаптивного порога и задержки выключения (hold‑time).
/// </summary>
public interface IVoiceActivityDetector
{
    /// <summary>
    /// Анализирует PCM-кадр (16 бит, моно) и возвращает true, если кадр должен быть отправлен.
    /// </summary>
    bool IsVoice(byte[] pcmData);

    /// <summary>
    /// Сбрасывает внутреннее состояние (при старте нового разговора).
    /// </summary>
    void Reset();
}
