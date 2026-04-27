using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении обновлённой информации о комнате
/// </summary>
public sealed class RoomInfoChangedEvent : BaseEvent
{
    /// <summary>
    /// Информация о комнате
    /// </summary>
    public RoomInfo Room { get; init; } = null!;
}
