using MIN.Core.Events.Contracts.Models;
using MIN.Discovery.Services.Contracts.Models;

namespace MIN.Discovery.Events;

/// <summary>
/// Событие, возникающее при нахождении комнаты в сети
/// </summary>
public sealed record RoomDiscoveredEvent : BaseEvent
{
    /// <summary>
    /// Информация о найденных комнатах
    /// </summary>
    public List<RoomDiscoveryInfo> RoomDiscoveryInfos { get; init; } = null!;
}
