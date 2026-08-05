namespace MIN.Core.Events.Contracts.Interfaces;

/// <summary>
/// Событие, публикуемое только внутри комнаты
/// </summary>
public interface IRoomScopedEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    Guid RoomId { get; }
}
