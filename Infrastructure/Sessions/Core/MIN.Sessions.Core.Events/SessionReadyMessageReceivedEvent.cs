using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// Получена информация о готовой сессии в комнате
/// </summary>
public sealed record SessionReadyMessageReceivedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Полученная информация о сессии
    /// </summary>
    public SessionReadyMessage Message { get; init; } = null!;
}
