using MIN.Entities;
using MIN.Events.Base;

namespace MIN.Events.Events;

/// <summary>
/// Событие, возникающее при изменении состояния комнаты
/// </summary>
public sealed class RoomStateChangedEvent : BaseEvent
{
    /// <summary>
    /// Информация о комнате
    /// </summary>
    public Room Room { get; init; } = null!;

    /// <summary>
    /// Предыдущее состояние активности
    /// </summary>
    public bool WasActive { get; init; }
}
