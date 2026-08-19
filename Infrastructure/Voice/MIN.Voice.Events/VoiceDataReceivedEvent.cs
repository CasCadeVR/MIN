using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Voice.Events;

/// <summary>
/// Получена хоть какие то звуки
/// </summary>
public sealed record VoiceDataReceivedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// От кого были получены звуки
    /// </summary>
    public Guid ParticipantId { get; init; }
}
