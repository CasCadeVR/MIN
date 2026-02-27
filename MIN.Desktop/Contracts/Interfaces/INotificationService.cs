using MIN.Services.Contracts.Models.Messages;

namespace MIN.Desktop.Contracts.Interfaces
{
    /// <summary>
    /// Сервис для уведомлений
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Отправить уведомление
        /// </summary>
        void Notify(ChatMessage message);
    }
}
