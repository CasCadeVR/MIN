namespace MIN.Voice.Services.Contacts.Interfaces;

/// <summary>
/// Сервис воспроизведения звука
/// </summary>
public interface IVoicePlaybackService : IDisposable
{
    /// <summary>
    /// Добавить канал звука для участника
    /// </summary>
    void AddParticipant(Guid participantId);

    /// <summary>
    /// Удалить канал звука для участника
    /// </summary>
    void RemoveParticipant(Guid participantId);

    /// <summary>
    /// Проиграть звук (реордер + декод + BufferedWaveProvider)
    /// </summary>
    void PlaySamples(Guid participantId, long sequenceNumber, byte[] data);

    /// <summary>
    /// Очистить все каналы звуков
    /// </summary>
    void Clear();
}
