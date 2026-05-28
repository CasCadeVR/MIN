namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис, способный связать комнату с её соединением в сети
/// </summary>
public interface IRoomConnectionRelated
{
    /// <summary>
    /// Получить идентификатор соединения для комнаты
    /// </summary>
    Guid GetConnectionIdByRoomId(Guid roomId);

    /// <summary>
    /// Получить идентификатор комнаты для соединения
    /// </summary>
    Guid GetRoomIdByConnectionId(Guid connectionId);
}
