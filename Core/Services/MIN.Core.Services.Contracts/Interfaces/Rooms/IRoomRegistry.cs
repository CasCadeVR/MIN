using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;

namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Реестр комнат и участников в ней
/// </summary>
public interface IRoomRegistry
{
    /// <summary>
    /// Получить комнату по идентификатору
    /// </summary>
    Room GetRoom(Guid roomId);

    /// <summary>
    /// Попытаться получить комнату по идентификатору
    /// </summary>
    bool TryGetRoom(Guid roomId, out Room room);

    /// <summary>
    /// Зарегистрировать комнату и связать её с идентификатором соединения
    /// </summary>
    void RegisterRoom(Guid connectionId, Room room);

    /// <summary>
    /// Удалить комнату по идентификатору соединения
    /// </summary>
    void UnregisterRoom(Guid connectionId);

    /// <summary>
    /// Добавить участника в комнату
    /// </summary>
    void AddParticipant(Guid roomId, ParticipantInfo participant);

    /// <summary>
    /// Удалить участника из комнаты
    /// </summary>
    void RemoveParticipant(Guid roomId, Guid participantId);

    /// <summary>
    /// Получить идентификатор комнаты по идентификатору соединения
    /// </summary>
    Guid GetRoomIdByConnectionId(Guid connectionId);

    /// <summary>
    /// Получить все комнаты
    /// </summary>
    IEnumerable<Room> GetAllRooms();

    /// <summary>
    /// Получить список участников комнаты
    /// </summary>
    IEnumerable<ParticipantInfo> GetCurrentParticipants(Guid roomId);
}
