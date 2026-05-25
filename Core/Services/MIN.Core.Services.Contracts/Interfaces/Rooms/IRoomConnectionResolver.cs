namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис, способный связать комнату с её соединением в сети
/// </summary>
public interface IRoomConnectionResolver
{
    /// <summary>
    /// Получить идентификатор комнаты для соединения
    /// </summary>
    Guid GetRoomIdByConnectionId(Guid connectionId, Guid? serverConnectionId);
}
