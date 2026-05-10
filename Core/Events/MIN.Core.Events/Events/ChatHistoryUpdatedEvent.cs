using MIN.Core.Events.Contracts;
using MIN.Core.Messaging.Stateless.RoomRelated.History;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении ответа на подгрузку сообщений
/// </summary>
public class ChatHistoryUpdatedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Сообщение ответа о подгрузки сообщений
    /// </summary>
    public ChatHistoryResponseMessage Message { get; init; } = null!;
}
