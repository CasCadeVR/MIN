using MIN.Core.Events.Contracts;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// События получения ответа от присоединения к сессии
/// </summary>
public sealed class SessionJoinResponseReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
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
