using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Chat.Events;

/// <summary>
/// Получено новое сообщение в комнате
/// </summary>
public sealed record ChatTextMessageReceivedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Полученное сообщение
    /// </summary>
    public ChatTextMessage Message { get; init; } = null!;

    /// <summary>
    /// Отправитель сообщения
    /// </summary>
    public ParticipantInfo Sender { get; init; } = null!;
}
