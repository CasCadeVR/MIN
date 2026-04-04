using MIN.Core.Events.Contracts;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при выходе участника из комнаты
/// </summary>
public sealed class ParticipantLeftEvent : BaseEvent
{
    /// <summary>
    /// Сообщение о вышедшем участнике
    /// </summary>
    public ParticipantLeftMessage Message { get; init; } = null!;
}
