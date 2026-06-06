using MIN.Core.Events.Contracts;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// События деактивации сервера сессии
/// </summary>
public sealed class SessionDeactivatedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; init; }
}
