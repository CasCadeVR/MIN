using MIN.Common.Core.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;

namespace MIN.Desktop.Views.Forms.HelperForms;

/// <summary>
/// Форма уведомления
/// </summary>
public partial class NotificationForm : StyledForm
{
    private readonly IDescribable describable;
    private readonly string roomName;
    private System.Windows.Forms.Timer appearTimer = null!;
    private System.Windows.Forms.Timer disappearTimer = null!;
    private System.Windows.Forms.Timer closeTimer = null!;

    private const int ANIMATION_INTERVAL_MS = 16;
    private const int ANIMATION_DURATION_MS = 400;
    private const int DISPLAY_DURATION_MS = 5000;

    private int animationSteps;
    private int totalSteps;
    private Point finalLocation;

    /// <summary>
    /// Нажато на отписку от уведомлений
    /// </summary>
    public event Action? NotificationTurnOffClicked;

    /// <summary>
    /// Нажато на уведомление
    /// </summary>
    public event Action? NotificationClicked;

    private bool isClosing;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NotificationForm"/>
    /// </summary>
    public NotificationForm(IDescribable describable, string roomName)
    {
        InitializeComponent();
        this.describable = describable;
        this.roomName = roomName;

        SetupNotificationProperties();
        FillFields();
        InitializeTimers();
        InitializeEvents();
    }

    private void SetupNotificationProperties()
    {
        ShowInTaskbar = false;
        TopMost = true;
        FormBorderStyle = FormBorderStyle.None;
        Opacity = 0;
        StartPosition = FormStartPosition.Manual;
    }

    private void InitializeEvents()
    {
        tableLayoutPanel.MouseClick += (_, _) => OnClicked();
        roomNameLabel.MouseClick += (_, _) => OnClicked();
        senderAndContent.MouseClick += (_, _) => OnClicked();
    }

    private void OnClicked()
    {
        Close();
        NotificationClicked?.Invoke();
    }

    private void InitializeTimers()
    {
        appearTimer = new System.Windows.Forms.Timer { Interval = ANIMATION_INTERVAL_MS };
        appearTimer.Tick += AppearTimer_Tick!;

        disappearTimer = new System.Windows.Forms.Timer { Interval = ANIMATION_INTERVAL_MS };
        disappearTimer.Tick += DisappearTimer_Tick!;

        closeTimer = new System.Windows.Forms.Timer { Interval = DISPLAY_DURATION_MS };
        closeTimer.Tick += (_, _) => CloseNotification();
    }

    private void FillFields()
    {
        roomNameLabel.Text = $"Комната {roomName}";
        senderAndContent.Text = describable.GetDescription();
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        tableLayoutPanel.BackColor = ColorScheme.IncomingMessageBackground;
        senderAndContent.Font = FontScheme.Monospace;
        roomNameLabel.Font = FontScheme.Caption;
    }

    /// <summary>
    /// Менеджер задает позицию, мы просто анимируем появление
    /// </summary>
    public void StartAppearAnimation(Point targetLocation)
    {
        finalLocation = targetLocation;
        Location = new Point(finalLocation.X, finalLocation.Y + 50);

        totalSteps = ANIMATION_DURATION_MS / ANIMATION_INTERVAL_MS;
        animationSteps = 0;
        appearTimer.Start();
    }

    private void AppearTimer_Tick(object sender, EventArgs e)
    {
        animationSteps++;
        var progress = (double)animationSteps / totalSteps;

        Opacity = progress;
        var currentY = finalLocation.Y + (int)(50 * (1 - progress));
        Location = new Point(finalLocation.X, currentY);

        if (animationSteps >= totalSteps)
        {
            appearTimer.Stop();
            Opacity = 1.0;
            Location = finalLocation;
            closeTimer.Start();
        }
    }

    private void StartDisappearAnimation()
    {
        if (isClosing)
        {
            return;
        }
        finalLocation = Location;
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
        var progress = (double)animationSteps / totalSteps;

        Opacity = 1.0 - progress;

        var currentY = finalLocation.Y + (int)(50 * progress);
        Location = new Point(finalLocation.X, currentY);

        if (animationSteps >= totalSteps)
        {
            disappearTimer.Stop();
            disappearTimer.Dispose();
            Close();
            Dispose();
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

    /// <summary>
    /// <inheritdoc cref="Form.OnFormClosed(FormClosedEventArgs)"/>
    /// </summary>
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
