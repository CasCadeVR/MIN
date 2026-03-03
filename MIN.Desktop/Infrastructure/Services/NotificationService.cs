using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Views.Forms.HelperForms;
using MIN.Services.Contracts.Models.Messages;

namespace MIN.Desktop.Infrastructure.Services
{
    /// <inheritdoc cref="INotificationService"/>
    public class NotificationService : INotificationService
    {
        private static readonly List<NotificationForm> activeNotifications = new List<NotificationForm>();
        private static readonly object _lock = new object();

        public event Action? OnNotificationClick;
        public event Action? NotificationTurnOffClicked;

        void INotificationService.Notify(ChatMessage message, string roomName)
        {
            if (Application.OpenForms.Count == 0) return;

            Form mainForm = Application.OpenForms[0];

            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(new Action(() => CreateAndShow(message, roomName)));
            }
            else
            {
                CreateAndShow(message, roomName);
            }
        }

        private void CreateAndShow(ChatMessage message, string roomName)
        {
            lock (_lock)
            {
                var notification = new NotificationForm(message, roomName);

                Screen screen = Screen.FromPoint(Cursor.Position);
                if (screen == null) screen = Screen.PrimaryScreen;

                int baseX = screen.WorkingArea.Right - 10;
                int baseY = screen.WorkingArea.Bottom - 10;

                activeNotifications.Add(notification);

                RepositionAll(screen, baseX, baseY);

                notification.FormClosed += (s, e) =>
                {
                    lock (_lock)
                    {
                        activeNotifications.Remove(notification);
                        RepositionAll(screen, baseX, baseY);
                    }
                };
                notification.NotificationTurnOffClicked += NotificationTurnOffClicked;
                notification.NotificationClicked += OnNotificationClick;
                notification.Show();
                notification.StartAppearAnimation(notification.Location);
            }
        }

        private static void RepositionAll(Screen screen, int baseX, int baseY)
        {
            int offsetY = 0;

            foreach (var notification in activeNotifications)
            {
                int targetY = baseY - notification.Height - offsetY - 10;
                int targetX = baseX - notification.Width;
                notification.Location = new Point(targetX, targetY);
                offsetY += notification.Height + 10;
            }
        }
    }
}
