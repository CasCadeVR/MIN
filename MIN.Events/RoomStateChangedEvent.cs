using MIN.Events.Base;
using MIN.Messaging.Contracts.Entities;

namespace MIN.Events;

/// <summary>
/// Событие, возникающее при изменении состояния комнаты
/// </summary>
public sealed class RoomStateChangedEvent : BaseEvent
{
    /// <summary>
    /// Информация о комнате
    /// </summary>
    public RoomInfo Room { get; init; } = null!;

    /// <summary>
    /// Предыдущее состояние активности
    /// </summary>
    public bool WasActive { get; init; }
}
