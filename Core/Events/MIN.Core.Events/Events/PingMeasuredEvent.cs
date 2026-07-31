using MIN.Core.Events.Contracts;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при измерения нового значения пинга
/// </summary>
public sealed class PingMeasuredEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Измеренный пинг (мс)
    /// </summary>
    public int PingMs { get; set; }
}
