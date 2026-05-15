using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts;

namespace MIN.Chat.Events;

/// <summary>
/// Событие, возникающее при смене статуса участника
/// </summary>
public sealed class OnlineStatusChangedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Статус действия в сети
    /// </summary>
    public OnlineStatus Status { get; set; }

    /// <summary>
    /// Идентификатор участника, сменивший статус
    /// </summary>
    public Guid SenderId { get; init; }
}
