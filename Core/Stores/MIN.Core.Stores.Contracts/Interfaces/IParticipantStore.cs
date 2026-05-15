using MIN.Core.Entities;

namespace MIN.Core.Stores.Contracts.Interfaces;

/// <summary>
/// Хранилище участников для комнаты
/// </summary>
public interface IParticipantStore
{
    /// <summary>
    /// Добавить участника в комнату
    /// </summary>
    void AddParticipant(Participant participant);

    /// <summary>
    /// Обновить участника
    /// </summary>
    void UpdateParticipant(Guid id, Participant participant);

    /// <summary>
    /// Удалить участника из комнаты
    /// </summary>
    void RemoveParticipant(Guid participantId);

    /// <summary>
    /// Получить участника комнаты
    /// </summary>
    Participant GetParticipantById(Guid participantId);

    /// <summary>
    /// Попытаться получить участника комнаты
    /// </summary>
    bool TryGetParticipantById(Guid participantId, out Participant? participant);

    /// <summary>
    /// Получить список всех участников комнаты
    /// </summary>
    IEnumerable<Participant> GetParticipants();

    /// <summary>
    /// Очистить участников для комнаты
    /// </summary>
    void ClearParticipants();
}
