using MIN.Core.Entities.Contracts.Models;

namespace MIN.Core.Stores.Contracts.Interfaces;

/// <summary>
/// Хранилище участников для комнаты
/// </summary>
public interface IParticipantStore
{
    /// <summary>
    /// Добавить участника в комнату
    /// </summary>
    void AddParticipant(Guid roomId, ParticipantInfo participant);

    /// <summary>
    /// Удалить участника из комнаты
    /// </summary>
    void RemoveParticipant(Guid roomId, Guid participantId);

    /// <summary>
    /// Получить участника комнаты
    /// </summary>
    ParticipantInfo GetParticipantById(Guid roomId, Guid participantId);

    /// <summary>
    /// Попытаться получить участника комнаты
    /// </summary>
    bool TryGetParticipantById(Guid roomId, Guid participantId, out ParticipantInfo? participantInfo);

    /// <summary>
    /// Получить список всех участников комнаты
    /// </summary>
    IEnumerable<ParticipantInfo> GetParticipants(Guid roomId);

    /// <summary>
    /// Очистить участников для комнаты
    /// </summary>
    void ClearParticipants(Guid roomId);
}
