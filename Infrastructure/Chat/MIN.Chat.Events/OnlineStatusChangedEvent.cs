using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Chat.Events;

/// <summary>
/// Событие, возникающее при смене статуса участника
/// </summary>
public sealed record OnlineStatusChangedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Статус действия в сети
    /// </summary>
    public required OnlineStatus Status { get; set; }

    /// <summary>
    /// Участник, сменивший статус
    /// </summary>
    public required ParticipantInfo Participant { get; init; }
}
