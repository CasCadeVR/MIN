using MIN.Core.Transport.Contracts.Models;

namespace MIN.Core.Entities;

/// <summary>
/// Локальные настройки комнаты
/// </summary>
public record LocalRoomSettings
{
    /// <summary>
    /// Включены ли уведомления
    /// </summary>
    public bool NotificationsEnabled { get; set; }

    /// <summary>
    /// Настройки глобальности сети
    /// </summary>
    public NetworkOptions NetworkOptions { get; set; }
}
