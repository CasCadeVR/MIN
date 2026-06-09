using MIN.Sessions.Core.Messaging.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class SessionResolver : ISessionResolver
{
    private readonly Dictionary<SessionType, Session> implementedSessions;

    /// <summary>
    /// Инициализирует новый экзепмляр <see cref="SessionMonitor"/>
    /// </summary>
    public SessionResolver(IEnumerable<ISessionPresenter> presenters)
    {
        implementedSessions = presenters.Select(x => x.GetSession()).ToDictionary(x => x.SessionType);
    }


    Session ISessionResolver.GetSessionByType(SessionType sessionType)
        => implementedSessions.TryGetValue(sessionType, out var session) ? session : throw new NotImplementedException(nameof(sessionType));
}
