using MIN.Core.Entities;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении детальной информации о комнате
/// </summary>
public sealed record RoomStateChangedEvent : BaseEvent, IRoomScopedEvent
{
    Guid IRoomScopedEvent.RoomId => Room.Id;

    /// <summary>
    /// Информация о комнате
    /// </summary>
    public Room Room { get; init; } = null!;
}
