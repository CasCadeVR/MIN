using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Voice.Events;

/// <summary>
/// Получена информация о смене состояния микрофона участника
/// </summary>
public sealed record VoiceMuteStateChangedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Его состояние микрофона
    /// </summary>
    public bool Muted { get; init; }

    /// <summary>
    /// У кого сменился состояние микрофона
    /// </summary>
    public Guid ParticipantId { get; init; }
}
