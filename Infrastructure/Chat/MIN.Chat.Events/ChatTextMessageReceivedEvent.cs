using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;

namespace MIN.Chat.Events;

/// <summary>
/// Получено новое сообщение в комнате
/// </summary>
public sealed class ChatTextMessageReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
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
