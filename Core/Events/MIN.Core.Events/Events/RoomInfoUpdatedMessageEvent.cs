using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении обновлённой информации о комнате
/// </summary>
public sealed record RoomInfoUpdatedMessageEvent : BaseEvent, IRoomScopedEvent
{
    Guid IRoomScopedEvent.RoomId => RoomInfo.Id;

    /// <summary>
    /// Информация о комнате
    /// </summary>
    public RoomInfo RoomInfo { get; init; } = null!;
}
