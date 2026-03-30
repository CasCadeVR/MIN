namespace MIN.Core.Events.Contracts;

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
