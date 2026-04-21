using MIN.Core.Events.Contracts;

namespace MIN.Desktop.Infrastructure.Events;

/// <summary>
/// Событие, возникающее при закрытии комнаты как хост
/// </summary>
public sealed class RoomClosedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }
}
