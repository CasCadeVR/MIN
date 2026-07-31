using MIN.Core.Entities.Contracts.Enums;

namespace MIN.Core.Services.Contracts.Models;

/// <summary>
/// Контекст отслеживания состояния подключения к комнате
/// </summary>
/// <param name="Role">Роль (что нужно делать при таймауте)</param>
/// <param name="RoomId">Идентификатор комнаты</param>
/// <param name="ConnectionId">Идентификатор соединения</param>
public record struct PingContext(Role Role, Guid RoomId, Guid ConnectionId) { }
