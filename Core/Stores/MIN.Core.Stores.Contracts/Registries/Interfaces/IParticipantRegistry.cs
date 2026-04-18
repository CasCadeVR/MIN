using MIN.Core.Entities.Contracts.Models;

namespace MIN.Core.Stores.Contracts.Registries.Interfaces;

/// <summary>
/// Сервис по ассоциации участников и соеднинений
/// </summary>
public interface IParticipantConnectionRegistry
{
    /// <summary>
    /// Установить ассоциацию соеднинения с участником
    /// </summary>
    void Register(Guid roomId, Guid connectionId, ParticipantInfo participant);

    /// <summary>
    /// Установить ассоциацию соеднинения с локальным участником
    /// </summary>
    void RegisterLocalParticipant(Guid roomId, ParticipantInfo participant);

    /// <summary>
    /// Получить данные участника от его соединения
    /// </summary>
    ParticipantInfo? GetParticipant(Guid roomId, Guid connectionId);

    /// <summary>
    /// Попытаться получить данные участника от его соединения
    /// </summary>
    bool TryGetParticipantFromConnectionId(Guid roomId, Guid connectionId, out ParticipantInfo participant);

    /// <summary>
    /// Получить идентификтор участника от идентификатора соеднинения
    /// </summary>
    Guid GetParticipantIdFromConnectionId(Guid roomId, Guid connectionId);

    /// <summary>
    /// Получить идентификтор соеднинения от идентификатора участника
    /// </summary>
    Guid GetConnectionIdFromParticipantId(Guid roomId, Guid participantId);

    /// <summary>
    /// Попытаться получить идентификатор соединения от идентификатора участника
    /// </summary>
    bool TryGetConnectionIdFromParticipantId(Guid roomId, Guid participantId, out Guid connectionId);

    /// <summary>
    /// Разорвать ассоциацию соеднинения с участником
    /// </summary>
    void Unregister(Guid roomId, Guid connectionId);
}
