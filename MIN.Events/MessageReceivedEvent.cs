using MIN.Events.Base;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.RoomRelated.ParticipantRelated;

namespace MIN.Events;

/// <summary>
/// Получено новое сообщение в комнате
/// </summary>
public sealed class MessageReceivedEvent : BaseEvent
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
