using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Desktop.Views.Forms.HelperForms;

/// <summary>
/// Форма создания комнаты
/// </summary>
public partial class SessionChoosingForm : StyledForm
{
    private readonly IEnumerable<Session> downloadedSessions;
    private Session? SelectedSession { get; set; }

    /// <summary>
    /// Событие по нажатию на кнопку
    /// </summary>
    public Action<Session>? OnSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionChoosingForm"/>
    /// </summary>
    public SessionChoosingForm(IEnumerable<Session> downloadedSessions)
    {
        InitializeComponent();
        this.downloadedSessions = downloadedSessions;
        InitializeImplementedSessions();
    }

    private void InitializeImplementedSessions()
    {
        foreach (var session in downloadedSessions)
        {
            var card = new SessionCard(session);
            card.OnClicked += (selected) =>
            {
                foreach (SessionCard otherCard in flowPanel.Controls.OfType<SessionCard>())
                {
                    if (otherCard.Session.SessionId != session.SessionId)
                    {
                        otherCard.UnselectCard();
                    }
                }
                SelectedSession = session;
                selectButton.Enabled = selected;
            };
            flowPanel.Controls.Add(card);
        }

        var exampleCard = flowPanel.Controls.OfType<SessionCard>().First();
        exampleCard.SelectAsDefault();

        var cardEffectiveWidth = exampleCard.Width + exampleCard.Margin.Horizontal;
        var cardEffectiveHeight = exampleCard.Height + exampleCard.Margin.Vertical;

        var downloadedSessionsCount = downloadedSessions.Count();

        var cols = Math.Min(downloadedSessionsCount, 3);
        var rows = Math.Min((int)Math.Ceiling(downloadedSessionsCount / 3.0), 2);

        var panelWidth = cols * cardEffectiveWidth + flowPanel.Padding.Horizontal;
        var panelHeight = rows * cardEffectiveHeight + flowPanel.Padding.Vertical;

        ClientSize = new Size(
            panelWidth + (Width - flowPanel.Width),
            panelHeight + (Height - flowPanel.Height)
        );

        MinimumSize = ClientSize;
        MaximumSize = ClientSize;

        if (downloadedSessionsCount > 6)
        {
            flowPanel.AutoScroll = true;
        }
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        Title.ForeColor = ColorScheme.TextOnAccent;
    }

    private void selectButton_Click(object sender, EventArgs e)
    {
        if (SelectedSession != null)
        {
            selectButton.Enabled = false;
            OnSelected?.Invoke(SelectedSession);
        }
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        Close();
    }
}
