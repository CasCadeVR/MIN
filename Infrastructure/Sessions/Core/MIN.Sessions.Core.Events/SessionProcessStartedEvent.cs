using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// События начала запуска сессии
/// </summary>
public sealed record SessionProcessStartedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; init; }

    /// <summary>
    /// Запускаемая сессия
    /// </summary>
    public required Session Session { get; init; }
}
