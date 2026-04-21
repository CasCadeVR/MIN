using MIN.Core.Events.Contracts;

namespace MIN.Desktop.Infrastructure.Events;

/// <summary>
/// Событие, возникающее при заходе в комнату как клиент
/// </summary>
public sealed class RoomJoinedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }
}
