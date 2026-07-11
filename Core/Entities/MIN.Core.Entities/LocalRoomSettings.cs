namespace MIN.Core.Entities;

/// <summary>
/// Локальные настройки комнаты
/// </summary>
public record struct LocalRoomSettings
{
    /// <summary>
    /// Включены ли уведомления
    /// </summary>
    public bool NotificationsEnabled { get; set; }
}
