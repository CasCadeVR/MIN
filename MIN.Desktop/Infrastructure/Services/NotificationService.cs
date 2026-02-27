using MIN.Desktop.Contracts.Interfaces;
using MIN.Services.Contracts.Models.Messages;

namespace MIN.Desktop.Infrastructure.Services
{
    /// <inheritdoc cref="INotificationService"/>
    public class NotificationService : INotificationService
    {
        //private void InitializeNotifications()
        //{
        //    var config = new NotifierConfiguration
        //    {
        //        // Позиция уведомления (правый нижний угол)
        //        PositionProvider = new WindowPositionProvider(
        //            Anchor.AnchorBottomRight,
        //            Offset = new Offset(10, 10)
        //        ),
        //        // Поведение при закрытии приложения
        //        LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
        //            notificationLifetime: TimeSpan.FromSeconds(5),
        //            maximumNotificationCount: MaximumNotificationCount.FromCount(3)
        //        ),
        //        // Дисплей (где показывать)
        //        DisplayOptions = new DisplayOptions()
        //    };

        //    notifier = new Notifier(config);
        //}

        void INotificationService.Notify(ChatMessage message)
        {
            MessageBox.Show(message.Content, $"От: {message.SenderName}");
        }
    }
}
