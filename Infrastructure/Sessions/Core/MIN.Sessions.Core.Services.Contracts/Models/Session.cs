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
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание сессии
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Путь к серверу сессии
    /// </summary>
    public string ServerPath { get; set; } = string.Empty;

    /// <summary>
    /// Путь к клиенту сессии
    /// </summary>
    public string ClientPath { get; set; } = string.Empty;
}
