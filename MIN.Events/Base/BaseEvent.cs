using MIN.Events.Contracts;

namespace MIN.Events.Base;

/// <summary>
/// Базовый класс для событий, реализующий IEvent
/// </summary>
public abstract class BaseEvent : IEvent
{
    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime CreationTime { get; } = DateTime.UtcNow;
}
