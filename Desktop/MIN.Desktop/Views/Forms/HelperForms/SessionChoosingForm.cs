using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Sessions.Core.DI.FeatureCollection;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Desktop.Views.Forms.HelperForms;

/// <summary>
/// Форма создания комнаты
/// </summary>
public partial class SessionChoosingForm : StyledForm
{
    private Session? SelectedSession { get; set; }

    /// <summary>
    /// Событие по нажатию на кнопку
    /// </summary>
    public Action<Session>? OnSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionChoosingForm"/>
    /// </summary>
    public SessionChoosingForm(ISessionFeatureCollection sessionFeatureCollection)
    {
        InitializeComponent();
        InitializeImplementedSessions(sessionFeatureCollection);
    }

    private void InitializeImplementedSessions(ISessionFeatureCollection sessionFeatureCollection)
    {
        var downloadedSessions = sessionFeatureCollection.SessionScanner.DownloadedSessions.Values;

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

        foreach (SessionCard card in flowPanel.Controls.OfType<SessionCard>())
        {
            var wantedWidth = card.Width + flowPanel.Padding.Horizontal * 3;
            Width = wantedWidth;
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
