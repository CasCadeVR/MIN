using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Services.Contracts.Interfaces;

/// <summary>
/// Презентер сессии
/// </summary>
public interface ISessionPresenter
{
    /// <summary>
    /// Получить описание сессии
    /// </summary>
    Session GetSession();
}
