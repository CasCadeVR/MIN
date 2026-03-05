using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Services.Contracts.Models.Messages;

namespace MIN.Desktop.Views.Forms.HelperForms
{
    public partial class NotificationForm : StyledForm
    {
        private readonly ChatMessage message;
        private readonly string messageRoomName;
        private System.Windows.Forms.Timer appearTimer;
        private System.Windows.Forms.Timer disappearTimer;
        private System.Windows.Forms.Timer closeTimer;

        private const int ANIMATION_INTERVAL_MS = 16;
        private const int ANIMATION_DURATION_MS = 400;
        private const int DISPLAY_DURATION_MS = 5000;

        private int animationSteps = 0;
        private int totalSteps = 0;
        private Point finalLocation;

        /// <summary>
        /// Нажато на отписку от уведомлений
        /// </summary>
        public event Action? NotificationTurnOffClicked;

        /// <summary>
        /// Нажато на уведомение
        /// </summary>
        public event Action? NotificationClicked;

        private bool isClosing = false;

        public NotificationForm(ChatMessage message, string messageRoomName)
        {
            InitializeComponent();
            this.message = message;
            this.messageRoomName = messageRoomName;

            SetupNotificationProperties();
            FillFields();
            InitializeTimers();
            InitializeEvents();
        }

        private void SetupNotificationProperties()
        {
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0;
            this.StartPosition = FormStartPosition.Manual;
        }

        private void InitializeEvents()
        {
            tableLayoutPanel.MouseClick += (_, _) => OnClicked();
            senderAndContent.MouseClick += (_, _) => OnClicked();
            roomName.MouseClick += (_, _) => OnClicked();
        }

        private void OnClicked()
        {
            Close();
            this.NotificationClicked?.Invoke();
        }

        private void InitializeTimers()
        {
            appearTimer = new System.Windows.Forms.Timer { Interval = ANIMATION_INTERVAL_MS };
            appearTimer.Tick += AppearTimer_Tick;

            disappearTimer = new System.Windows.Forms.Timer { Interval = ANIMATION_INTERVAL_MS };
            disappearTimer.Tick += DisappearTimer_Tick;

            closeTimer = new System.Windows.Forms.Timer { Interval = DISPLAY_DURATION_MS };
            closeTimer.Tick += (s, e) => CloseNotification();
        }

        private void FillFields()
        {
            roomName.Text = $"Комната {messageRoomName}";
            senderAndContent.Text = $"{message.SenderName}: {message.Content}";
        }

        protected override void ApplyStylings()
        {
            tableLayoutPanel.BackColor = ColorScheme.IncomingMessageBackground;
            logoName.ForeColor = ColorScheme.PrimaryAccent;
            roomName.ForeColor = ColorScheme.TextPrimary;
        }

        /// <summary>
        /// Менеджер задает позицию, мы просто анимируем появление
        /// </summary>
        public void StartAppearAnimation(Point targetLocation)
        {
            finalLocation = targetLocation;
            this.Location = new Point(finalLocation.X, finalLocation.Y + 50);

            totalSteps = ANIMATION_DURATION_MS / ANIMATION_INTERVAL_MS;
            animationSteps = 0;
            appearTimer.Start();
        }

        private void AppearTimer_Tick(object sender, EventArgs e)
        {
            animationSteps++;
            double progress = (double)animationSteps / totalSteps;

            this.Opacity = progress;
            int currentY = finalLocation.Y + (int)(50 * (1 - progress));
            this.Location = new Point(finalLocation.X, currentY);

            if (animationSteps >= totalSteps)
            {
                appearTimer.Stop();
                this.Opacity = 1.0;
                this.Location = finalLocation;
                closeTimer.Start();
            }
        }

        private void StartDisappearAnimation()
        {
            if (isClosing)
            {
                return;
            }
            isClosing = true;

            closeTimer.Stop();
            appearTimer.Stop();

            totalSteps = ANIMATION_DURATION_MS / ANIMATION_INTERVAL_MS;
            animationSteps = 0;
            disappearTimer.Start();
        }

        private void DisappearTimer_Tick(object sender, EventArgs e)
        {
            animationSteps++;
            double progress = (double)animationSteps / totalSteps;

            this.Opacity = 1.0 - progress;

            int currentY = finalLocation.Y + (int)(50 * progress);
            this.Location = new Point(finalLocation.X, currentY);

            if (animationSteps >= totalSteps)
            {
                disappearTimer.Stop();
                disappearTimer.Dispose();
                this.Close();
                this.Dispose();
            }
        }

        private void CloseNotification()
        {
            if (isClosing)
            {
                return;
            }
            StartDisappearAnimation();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            CloseNotification();
        }

        private void notificationTurnOff_Click(object sender, EventArgs e)
        {
            NotificationTurnOffClicked?.Invoke();
            CloseNotification();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            appearTimer?.Stop();
            appearTimer?.Dispose();
            disappearTimer?.Stop();
            disappearTimer?.Dispose();
            closeTimer?.Stop();
            closeTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
