using MIN.Core.Events.Contracts;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// События пересканирования папок скаченных сессий
/// </summary>
public sealed class SessionRescanCompletedEvent : BaseEvent
{
    /// <summary>
    /// Список скаченных сессий
    /// </summary>
    public required Dictionary<string, Session> DownloadedSessions { get; set; }
}
