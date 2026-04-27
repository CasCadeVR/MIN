using MIN.Core.Entities;
using MIN.Core.Events.Contracts;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении детальной информации о комнате
/// </summary>
public sealed class RoomStateChangedEvent : BaseEvent
{
    /// <summary>
    /// Информация о комнате
    /// </summary>
    public Room Room { get; init; } = null!;
}
