using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;

namespace MIN.Discovery.Events;

/// <summary>
/// Событие, возникающее при нахождении комнаты в сети
/// </summary>
public class RoomDiscoveredEvent : BaseEvent
{
    /// <summary>
    /// Информация о найденной комнате
    /// </summary>
    public RoomInfo Room { get; init; } = null!;
}
