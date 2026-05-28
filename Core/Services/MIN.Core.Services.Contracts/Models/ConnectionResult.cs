namespace MIN.Core.Services.Contracts.Models;

/// <summary>
/// Результат подключения к комнате
/// </summary>
/// <param name="RoomId">Идентификатор комнаты</param>
/// <param name="ConnectionId">Идентификатор подключения</param>
public record struct ConnectionResult(Guid RoomId, Guid ConnectionId) { }
