using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при измерения нового значения пинга
/// </summary>
public sealed record PingMeasuredEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Измеренный пинг (мс)
    /// </summary>
    public int PingMs { get; set; }
}
