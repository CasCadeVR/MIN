namespace MIN.Voice.Services.Contacts.Interfaces;

/// <summary>
/// Сервис для обработки своего и мут других участников
/// </summary>
public interface IMuteService
{
    /// <summary>
    /// Отключить микрофон
    /// </summary>
    Task MuteSelf(Guid roomId, int subroomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Включить микрофон
    /// </summary>
    Task UnmuteSelf(Guid roomId, int subroomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Замьютить участника
    /// </summary>
    void MuteParticipant(Guid participantId);

    /// <summary>
    /// Размьютить участника
    /// </summary>
    void UnmuteParticipant(Guid participantId);
}
