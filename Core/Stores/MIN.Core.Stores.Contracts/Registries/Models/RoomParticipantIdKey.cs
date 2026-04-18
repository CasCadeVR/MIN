namespace MIN.Core.Stores.Contracts.Registries.Models;

/// <summary>
/// Ключ для сохранения RoomId и ConnectionId
/// </summary>
/// <param name="RoomId">Идентификатор комнаты</param>
/// <param name="ParticipantId">Идентификатор участника</param>
public record struct RoomParticipantIdKey(Guid RoomId, Guid ParticipantId);
