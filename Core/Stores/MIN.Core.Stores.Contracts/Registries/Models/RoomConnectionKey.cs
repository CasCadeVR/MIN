namespace MIN.Core.Stores.Contracts.Registries.Models;

/// <summary>
/// Ключ для сохранения RoomId и ConnectionId
/// </summary>
/// <param name="RoomId">Идентификатор комнаты</param>
/// <param name="ConnectionId">Идентификатор соединения</param>
public record struct RoomConnectionKey(Guid RoomId, Guid ConnectionId);
