using MIN.Sessions.Core.Services.Contracts.Enums;

namespace MIN.Sessions.Core.Services.Contracts.Models;

/// <summary>
/// Сессия
/// </summary>
public class Session
{
    /// <summary>
    /// Тип сессии
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Название сессии
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание сессии
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
