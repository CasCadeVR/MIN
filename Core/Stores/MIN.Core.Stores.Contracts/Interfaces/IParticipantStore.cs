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
    void AddParticipant(ParticipantInfo participant);

    /// <summary>
    /// Удалить участника из комнаты
    /// </summary>
    void RemoveParticipant(Guid participantId);

    /// <summary>
    /// Получить участника комнаты
    /// </summary>
    ParticipantInfo GetParticipantById(Guid participantId);

    /// <summary>
    /// Попытаться получить участника комнаты
    /// </summary>
    bool TryGetParticipantById(Guid participantId, out ParticipantInfo? participantInfo);

    /// <summary>
    /// Получить список всех участников комнаты
    /// </summary>
    IEnumerable<ParticipantInfo> GetParticipants();

    /// <summary>
    /// Очистить участников для комнаты
    /// </summary>
    void ClearParticipants();
}
