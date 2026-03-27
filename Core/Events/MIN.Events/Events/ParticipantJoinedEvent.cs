using MIN.Events.Contracts;
using MIN.Entities.Contracts.Models;

namespace MIN.Events.Events;

/// <summary>
/// Событие, возникающее при входе участника в комнату
/// </summary>
public sealed class ParticipantJoinedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Информация о присоединившемся участнике
    /// </summary>
    public ParticipantInfo Participant { get; init; } = null!;
}
