using MIN.Events.Base;
using MIN.Messaging.Contracts.Entities;

namespace MIN.Events.Events;

/// <summary>
/// Пример события: новый участник присоединился к комнате
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
