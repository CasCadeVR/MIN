using MIN.Entities.Contracts.Models;
using MIN.Events.Contracts;

namespace MIN.Discovery.Events;

/// <summary>
/// Событие, возникающее при нахождении комнаты в сети
/// </summary>
public class RoomDiscoveredEvent : BaseEvent
{
    /// <summary>
    /// Информация о найденной комнате
    /// </summary>
    public RoomInfo Room { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomDiscoveredEvent"/>
    /// </summary>
    public RoomDiscoveredEvent(RoomInfo room)
    {
        Room = room;
    }
}
