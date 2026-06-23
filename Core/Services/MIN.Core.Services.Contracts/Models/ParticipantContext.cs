namespace MIN.Core.Services.Contracts.Models;

/// <summary>
/// Контекст участника в комнате (пока нужен только для обработки ошибок)
/// </summary>
/// <param name="RoomId">Идентификатор комнаты</param>
/// <param name="ParticipantId">Идентификатор участника</param>
public record struct ParticipantContext(Guid RoomId, Guid ParticipantId) { }
