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
    /// Изменить громкость определённого участника
    /// </summary>
    void ChangeParticipantVolume(Guid participantId, int specificVolume);

    /// <summary>
    /// Удалить канал звука для участника
    /// </summary>
    void RemoveParticipant(Guid participantId);

    /// <summary>
    /// Проиграть звук (реордер + декод + BufferedWaveProvider)
    /// </summary>
    void PlaySamples(Guid participantId, long sequenceNumber, byte[] data);

    /// <summary>
    /// Очистить все каналы звуков и выйти из подкомнаты звука
    /// </summary>
    void Clear();
}
