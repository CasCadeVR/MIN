using MIN.Desktop.Views.Forms.HelperForms;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Desktop.Infrastructure.Services
{
    /// <inheritdoc cref="INotificationService"/>
    public class NotificationService : INotificationService
    {
        private readonly static List<NotificationForm> activeNotifications = [];
        private readonly static object @lock = new();

        /// <summary>
        /// Событие по нажатию на уведомление
        /// </summary>
        public event Action? OnNotificationClick;

        /// <summary>
        /// Событие по нажатию на выключение уведомления
        /// </summary>
        public event Action? NotificationTurnOffClicked;

        void INotificationService.Notify(string message, string roomName, string? sender)
        {
            if (Application.OpenForms.Count == 0)
            {
                return;
            }

            var mainForm = Application.OpenForms[0];

            if (mainForm!.InvokeRequired)
            {
                mainForm.Invoke(new Action(() => CreateAndShow(message, roomName, sender)));
            }
            else
            {
                CreateAndShow(message, roomName, sender);
            }
        }

        private void CreateAndShow(string message, string roomName, string? sender)
        {
            lock (@lock)
            {
                var notification = new NotificationForm(message, roomName, sender);
                notification.NotificationTurnOffClicked += NotificationTurnOffClicked;
                notification.NotificationClicked += OnNotificationClick;

                var screen = Screen.FromPoint(Cursor.Position);
                screen ??= Screen.PrimaryScreen!;

                var baseX = screen.WorkingArea.Right - 10;
                var baseY = screen.WorkingArea.Bottom - 10;

                activeNotifications.Add(notification);

                RepositionAll(baseX, baseY);

                notification.FormClosed += (s, e) =>
                {
                    lock (@lock)
                    {
                        activeNotifications.Remove(notification);
                        RepositionAll(baseX, baseY);
                    }
                };

                notification.Show();
                notification.StartAppearAnimation(notification.Location);
            }
        }

        private static void RepositionAll(int baseX, int baseY)
        {
            var offsetY = 0;

            foreach (var notification in activeNotifications)
            {
                var targetY = baseY - notification.Height - offsetY - 10;
                var targetX = baseX - notification.Width;
                notification.Location = new Point(targetX, targetY);
                offsetY += notification.Height + 10;
            }
        }
    }
}
