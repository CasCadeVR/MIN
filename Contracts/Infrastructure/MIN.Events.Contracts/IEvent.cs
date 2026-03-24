namespace MIN.Events.Contracts;

/// <summary>
/// Базовый интерфейс для всех событий приложения
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Уникальный идентификатор события
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Время создания события (UTC)
    /// </summary>
    DateTime CreationTime { get; }
}
