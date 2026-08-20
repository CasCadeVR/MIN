using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Chat.Events;

/// <summary>
/// Сообщение было удалено
/// </summary>
public sealed record ChatMessageDeletedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор удалённого сообщения
    /// </summary>
    public Guid MessageId { get; init; }
}
