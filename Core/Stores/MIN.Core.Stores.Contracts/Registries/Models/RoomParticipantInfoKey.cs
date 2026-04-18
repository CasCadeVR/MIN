using MIN.Core.Entities.Contracts.Models;

namespace MIN.Core.Stores.Contracts.Registries.Models;

/// <summary>
/// Ключ для сохранения RoomId и ConnectionId
/// </summary>
/// <param name="RoomId">Идентификатор комнаты</param>
/// <param name="ParticipantInfo">Информация об участнике</param>
public record struct RoomParticipantInfoKey(Guid RoomId, ParticipantInfo ParticipantInfo);
