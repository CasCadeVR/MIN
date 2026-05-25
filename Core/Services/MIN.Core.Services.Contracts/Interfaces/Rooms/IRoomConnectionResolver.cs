namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис, способный связать комнату с её соединением в сети
/// </summary>
public interface IRoomConnectionResolver
{
    /// <summary>
    /// Получить идентификатор соединения хоста из соединения клиента и комнаты
    /// </summary>
    Guid? GetServerConnectionIdByRoomId(Guid connectionId, Guid roomId);

    /// <summary>
    /// Получить идентификатор комнаты для соединения
    /// </summary>
    Guid GetRoomIdByConnectionId(Guid connectionId, Guid? serverConnectionId);
}
