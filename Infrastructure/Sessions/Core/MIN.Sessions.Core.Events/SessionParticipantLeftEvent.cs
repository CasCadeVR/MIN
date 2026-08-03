using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// События выхода участника из сессии
/// </summary>
public sealed record SessionParticipantLeftEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; init; }

    /// <summary>
    /// Вышедший участник
    /// </summary>
    public ParticipantInfo Participant { get; init; } = null!;
}
