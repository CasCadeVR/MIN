using MIN.Core.Events.Contracts;

namespace MIN.Desktop.Infrastructure.Events;

/// <summary>
/// Событие, возникающее при получении нового сообщения в комнате
/// </summary>
public sealed class ChatMessageReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Отправитель сообщения
    /// </summary>
    public string? Sender { get; init; }
}
