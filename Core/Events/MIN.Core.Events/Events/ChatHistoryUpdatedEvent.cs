using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.Core.Messaging.Stateless.RoomRelated.History;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении ответа на подгрузку сообщений
/// </summary>
public sealed record ChatHistoryUpdatedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Сообщение ответа о подгрузки сообщений
    /// </summary>
    public ChatHistoryResponseMessage Message { get; init; } = null!;
}
