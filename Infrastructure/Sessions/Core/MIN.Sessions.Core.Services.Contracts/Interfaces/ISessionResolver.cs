using MIN.Sessions.Core.Messaging.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Services.Contracts.Interfaces;

/// <summary>
/// Презентер сессии
/// </summary>
public interface ISessionResolver
{
    /// <summary>
    /// Получить описание сессии по его типу
    /// </summary>
    Session GetSessionByType(SessionType sessionType);
}
