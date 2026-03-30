using MIN.Core.Entities.Contracts.Models;

namespace MIN.Core.Services.Contracts.Interfaces;

/// <summary>
/// Сервис по ассоциации участников и соеднинений
/// </summary>
public interface IParticipantRegistry
{
    /// <summary>
    /// Установить ассоциацию соеднинения с участником
    /// </summary>
    void SetParticipantInfo(Guid connectionId, ParticipantInfo participant);

    /// <summary>
    /// Получить данные участника от его соединения
    /// </summary>
    ParticipantInfo? GetParticipantInfo(Guid connectionId);

    /// <summary>
    /// Получить идентификтор соеднинения от идентификатора участника
    /// </summary>
    Guid GetConnectionIdFromParticipantId(Guid participantId);

    /// <summary>
    /// Попытаться получить данные участника от его соединения
    /// </summary>
    bool TryGetParticipantInfo(Guid connectionId, out ParticipantInfo participant);

    /// <summary>
    /// Разорвать ассоциацию соеднинения с участником
    /// </summary>
    void RemoveParticipantInfo(Guid connectionId);
}
