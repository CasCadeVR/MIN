using MIN.Core.Events.Contracts;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при входе участника в комнату
/// </summary>
public sealed class ParticipantJoinedEvent : BaseEvent
{
    /// <summary>
    /// Сообщение о присоединившимся участнике
    /// </summary>
    public ParticipantJoinedMessage Message { get; init; } = null!;
}
