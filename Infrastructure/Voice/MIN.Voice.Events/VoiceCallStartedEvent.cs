using MIN.Core.Entities;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.Voice.Messaging;

namespace MIN.Voice.Events;

/// <summary>
/// Получена информация о начале звонка в комнате
/// </summary>
public sealed record VoiceCallStartedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Полученная информация о начале звонка
    /// </summary>
    public VoiceCallStartedMessage Message { get; init; } = null!;

    /// <summary>
    /// Начавший звонок участник
    /// </summary>
    public Participant Participant { get; init; } = null!;
}
