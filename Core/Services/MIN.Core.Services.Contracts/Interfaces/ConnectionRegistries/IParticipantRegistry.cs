using MIN.Core.Entities.Contracts.Models;

namespace MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;

/// <summary>
/// Сервис по ассоциации участников и соеднинений
/// </summary>
public interface IParticipantConnectionRegistry
{
    /// <summary>
    /// Установить ассоциацию соеднинения с участником
    /// </summary>
    void Register(Guid connectionId, ParticipantInfo participant);

    /// <summary>
    /// Установить ассоциацию соеднинения с локальным участником
    /// </summary>
    void RegisterLocalParticipant(ParticipantInfo participant);

    /// <summary>
    /// Получить данные участника от его соединения
    /// </summary>
    ParticipantInfo? GetParticipant(Guid connectionId);

    /// <summary>
    /// Получить идентификтор соеднинения от идентификатора участника
    /// </summary>
    Guid GetConnectionIdFromParticipantId(Guid participantId);

    /// <summary>
    /// Попытаться получить данные участника от его соединения
    /// </summary>
    bool TryGetParticipantFromConnectionId(Guid connectionId, out ParticipantInfo participant);

    /// <summary>
    /// Разорвать ассоциацию соеднинения с участником
    /// </summary>
    void Unregister(Guid connectionId);
}
