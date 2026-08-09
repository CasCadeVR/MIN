using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.Voice.Messaging;

namespace MIN.Voice.Events;

/// <summary>
/// Получена информация о звонкк в комнате
/// </summary>
public sealed record VoiceCallStateReceivedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Полученная информация о звонке
    /// </summary>
    public VoiceCallStateResponseMessage Message { get; init; } = null!;
}
