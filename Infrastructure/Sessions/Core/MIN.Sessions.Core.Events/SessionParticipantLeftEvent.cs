using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// События выхода участника из сессии
/// </summary>
public sealed class SessionParticipantLeftEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
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
