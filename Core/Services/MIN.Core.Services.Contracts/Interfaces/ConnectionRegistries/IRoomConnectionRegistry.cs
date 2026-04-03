namespace MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;

/// <summary>
/// Сервис по ассоциации комнат и соеднинений
/// </summary>
public interface IRoomConnectionRegistry
{
    /// <summary>
    /// Связать комнату с идентификатором соединения
    /// </summary>
    void Associate(Guid connectionId, Guid roomId);

    /// <summary>
    /// Получить идентификатор комнаты по идентификатору соединения
    /// </summary>
    Guid GetRoomIdByConnectionId(Guid connectionId);

    /// <summary>
    /// Попробовать получить идентификатор комнаты по идентификатору соединения
    /// </summary>
    bool TryGetRoomId(Guid connectionId, out Guid roomId);

    /// <summary>
    /// Разорвать связь комнаты и её идентификатор соединения
    /// </summary>
    /// <param name="connectionId"></param>
    void Disassociate(Guid connectionId);
}
