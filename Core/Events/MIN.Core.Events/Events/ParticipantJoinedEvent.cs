using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при входе участника в комнату
/// </summary>
public sealed record ParticipantJoinedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Сообщение о присоединившимся участнике
    /// </summary>
    public ParticipantJoinedMessage Message { get; init; } = null!;
}
