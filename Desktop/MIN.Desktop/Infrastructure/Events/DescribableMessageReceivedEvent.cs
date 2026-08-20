using System;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Desktop.Infrastructure.Events;

/// <summary>
/// Событие, возникающее при получении описаемого сообщения в комнате
/// </summary>
public sealed record DescribableMessageReceivedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор описываемого сообщения
    /// </summary>
    public Guid MessageId { get; init; }

    /// <summary>
    /// Описаемое сообщение
    /// </summary>
    public IDescribable DescribableMessage { get; init; } = null!;
}
