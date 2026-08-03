using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при выходе участника из комнаты
/// </summary>
public sealed record ParticipantLeftEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Сообщение о вышедшем участнике
    /// </summary>
    public ParticipantLeftMessage Message { get; init; } = null!;
}
