using MIN.Core.Entities.Contracts.Models;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Models;

namespace MIN.Core.SubRooms.Contracts.Interfaces;

/// <summary>
/// Менеджер по подкомнатам
/// </summary>
public interface ISubRoomManager
{
    /// <summary>
    /// Захостить подкомнаты
    /// </summary>
    SubRoomInfo HostSubRoom(Guid roomId, Guid creatorId, SubRoomPurpose purpose);

    /// <summary>
    /// Попытаться войти в подкомнату
    /// </summary>
    bool TryJoinSubRoom(Guid roomId, int subRoomId, Guid participantId);

    /// <summary>
    /// Уйти из подкомнаты
    /// </summary>
    void LeaveSubRoom(Guid roomId, int subRoomId, Guid participantId);

    /// <summary>
    /// Попытаться остановить подкомнаты
    /// </summary>
    /// <remarks>
    /// Доступно только хосту или инициатору подкомнаты
    /// </remarks>
    bool TryStopSubRoom(Guid roomId, int subRoomId, Guid requesterId);

    /// <summary>
    /// Получить список участников подкомнаты
    /// </summary>
    IReadOnlyList<ParticipantInfo> GetParticipants(Guid roomId, int subRoomId);

    /// <summary>
    /// Получить информацию о подкомнате
    /// </summary>
    SubRoomInfo? GetSubRoom(Guid roomId, int subRoomId);

    /// <summary>
    /// Получить все подкомнаты комнаты
    /// </summary>
    IReadOnlyList<SubRoomInfo> GetRoomSubRooms(Guid roomId);
}
