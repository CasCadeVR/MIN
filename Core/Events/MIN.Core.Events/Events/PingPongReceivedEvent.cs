using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении Ping-Pong сообщения
/// </summary>
public sealed class PingPongReceivedEvent : BaseEvent
{
    /// <summary>
    /// Роль получения сообщения
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }
}
