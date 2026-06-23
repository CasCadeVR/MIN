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
    SubRoomInfo HostSubRoom(Guid roomId, ParticipantInfo creator, SubRoomPurpose purpose);

    /// <summary>
    /// Запустить подкомнату
    /// </summary>
    bool ActivateSubRoom(Guid roomId, int subRoomId, ParticipantInfo participant);

    /// <summary>
    /// Попытаться войти в подкомнату
    /// </summary>
    SubRoomJoinOutcome TryJoinSubRoom(Guid roomId, int subRoomId, ParticipantInfo participant);

    /// <summary>
    /// Находиться ли участник внутри подкомнаты
    /// </summary>
    bool IsInSubRoom(Guid roomId, int subRoomId, Guid participantId);

    /// <summary>
    /// Уйти из подкомнаты и деактивирует в случае выхода последнего участника
    /// </summary>
    /// <returns>
    /// true - если комната ещё активна
    /// false - если нет
    /// </returns>
    bool LeaveSubRoom(Guid roomId, int subRoomId, Guid participantId);

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
    IReadOnlyList<Guid> GetParticipantIds(Guid roomId, int subRoomId);

    /// <summary>
    /// Получить количество участников подкомнаты
    /// </summary>
    int GetParticipantCount(Guid roomId, int subRoomId);

    /// <summary>
    /// Получить информацию о подкомнате
    /// </summary>
    SubRoomInfo? GetSubRoom(Guid roomId, int subRoomId);

    /// <summary>
    /// Получить все подкомнаты комнаты
    /// </summary>
    IReadOnlyList<SubRoomInfo> GetRoomSubRooms(Guid roomId);

    /// <summary>
    /// Очистить все подкомнаты для комнаты
    /// </summary>
    void ClearRoomSubRooms(Guid roomId);
}
