namespace MIN.Voice.Services.Contacts.Interfaces;

/// <summary>
/// Сервис для обработки своего и мут других участников
/// </summary>
public interface IMuteService
{
    /// <summary>
    /// Отключить микрофон
    /// </summary>
    void MuteSelf();

    /// <summary>
    /// Включить микрофон
    /// </summary>
    void UnmuteSelf(Guid roomId, int subroomId);

    /// <summary>
    /// Замьютить участника
    /// </summary>
    void MuteParticipant(Guid participantId);

    /// <summary>
    /// Размьютить участника
    /// </summary>
    void UnmuteParticipant(Guid participantId);
}
