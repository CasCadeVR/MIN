using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// События получения ответа от присоединения к сессии
/// </summary>
public sealed record SessionJoinResponseReceivedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; init; }

    /// <summary>
    /// Сессия
    /// </summary>
    public Session Session { get; init; } = null!;
}
