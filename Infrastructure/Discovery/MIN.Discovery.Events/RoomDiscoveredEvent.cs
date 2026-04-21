using MIN.Core.Events.Contracts;
using MIN.Discovery.Services.Contracts.Models;

namespace MIN.Discovery.Events;

/// <summary>
/// Событие, возникающее при нахождении комнаты в сети
/// </summary>
public class RoomDiscoveredEvent : BaseEvent
{
    /// <summary>
    /// Имя компьютера в сети, где было получено сообщение
    /// </summary>
    public string MachineName { get; set; } = string.Empty;

    /// <summary>
    /// Информация о найденных комнатах
    /// </summary>
    public List<RoomDiscoveryInfo> RoomDiscoveryInfos { get; init; } = null!;
}
