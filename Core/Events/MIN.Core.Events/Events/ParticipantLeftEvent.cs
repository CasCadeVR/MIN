using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при выходе участника из комнаты
/// </summary>
public sealed class ParticipantLeftEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Информация о покинувшем участнике
    /// </summary>
    public ParticipantInfo Participant { get; init; } = null!;
}
