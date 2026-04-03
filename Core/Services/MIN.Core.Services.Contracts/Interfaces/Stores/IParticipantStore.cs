using MIN.Core.Entities.Contracts.Models;

namespace MIN.Core.Services.Contracts.Interfaces.Stores;

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
    /// Получить список всех участников комнаты
    /// </summary>
    IEnumerable<ParticipantInfo> GetParticipants(Guid roomId);

    /// <summary>
    /// Очистить участников для комнаты
    /// </summary>
    void ClearParticipants(Guid roomId);
}
