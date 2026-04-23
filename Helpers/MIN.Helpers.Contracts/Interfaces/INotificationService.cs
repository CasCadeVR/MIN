namespace MIN.Helpers.Contracts.Interfaces;

/// <summary>
/// Сервис для уведомлений
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Cобытие по нажатию на уведомление
    /// </summary>
    event Action OnNotificationClick;

    /// <summary>
    /// Cобытие по нажатию на отписку от увдомлений
    /// </summary>
    event Action NotificationTurnOffClicked;

    /// <summary>
    /// Отправить уведомление
    /// </summary>
    void Notify(string message, string roomName, string? sender = null);
}
