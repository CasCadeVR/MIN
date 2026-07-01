using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Winforms.Properties;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сессии
/// </summary>
public partial class SessionCard : UserControl
{
    private bool selected;

    /// <summary>
    /// Сессия
    /// </summary>
    public Session Session { get; init; }

    /// <summary>
    /// Событие по нажатию на карточку
    /// </summary>
    public Action<bool>? OnClicked { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionCard"/>
    /// </summary>
    public SessionCard(Session session)
    {
        InitializeComponent();
        ApplyStylings();
        Session = session;

        FillLabels();
    }

    /// <summary>
    /// Отменить выбор карточки
    /// </summary>
    public void UnselectCard()
    {
        selected = false;
        UpdateStylesOutOfSelected();
    }

    /// <summary>
    /// Выбрать как по умолчанию
    /// </summary>
    public void SelectAsDefault()
    {
        SelectCard();
        OnClicked?.Invoke(selected);
    }

    private void SelectCard()
    {
        selected = true;
        UpdateStylesOutOfSelected();
    }

    private void UpdateStylesOutOfSelected()
    {
        BackColor = selected
            ? ColorScheme.ChatPanelFileDropBackground
            : ColorScheme.SecondaryAccent;
    }

    private void ApplyStylings()
    {
        BackColor = ColorScheme.SecondaryAccent;
        tableLayoutPanelLabels.BackColor = ColorScheme.DefaultParticipantCardBackground;
    }

    private void FillLabels()
    {
        sessionName.Text = Session.Name;
        sessionDescription.Text = Session.Description;
        sessionVersion.Text += Session.Version.ToString();
        sessionMaximumParticipants.Text += Session.MaximumParticipants.HasValue
            ? Session.MaximumParticipants.Value
            : "Неограничено";

        if (Session.ThumbnailFileName == null)
        {
            sessionImage.Image = Resources.rocket;
        }
        else
        {
            var bytes = File.ReadAllBytes(Session.GeThumbnailPath());
            using var ms = new MemoryStream(bytes);
            using var tempImage = Image.FromStream(ms);
            sessionImage.Image = new Bitmap(tempImage);
        }
    }

    private void card_Click(object sender, EventArgs e)
    {
        if (selected)
        {
            UnselectCard();
        }
        else
        {
            SelectCard();
        }
        OnClicked?.Invoke(selected);
    }

    private void downloadLinkLabel_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(Session.DownloadLink);
            downloadLinkLabel.Text = "Скопировано!";
        }
        catch { }
    }
}
