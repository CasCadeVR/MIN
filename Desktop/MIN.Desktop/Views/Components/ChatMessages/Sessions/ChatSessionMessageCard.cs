using System.Diagnostics;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Properties;
using MIN.Desktop.Views.Components.ChatMessages;
using MIN.Sessions.Core.DI.FeatureCollection;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения, представляющая файл от пользователя
/// </summary>
public partial class ChatSessionMessageCard : BaseChatMessageCard, IDisposable
{
    private readonly IEventBus eventBus = null!;
    private readonly Guid roomId;
    private readonly SessionReadyMessage sessionReadyMessage = null!;
    private readonly SynchronizationContext uiContext = null!;
    private HashSet<IDisposable> eventTokens = null!;
    private bool asDownloaded;
    private int currentAmount;

    /// <summary>
    /// Событие, возникающее по нажатию на кнопку присоединиться
    /// </summary>
    public event Func<Task>? OnJoinRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileMessageCard"/>
    /// </summary>
    public ChatSessionMessageCard() : base()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileMessageCard"/>
    /// </summary>
    public ChatSessionMessageCard(ISessionFeatureCollection sessionFeatureCollection,
        IEventBus eventBus,
        Guid roomId,
        SessionReadyMessage sessionReadyMessage,
        ParticipantInfo localParticipant,
        bool isHostMessage,
        bool removeHeaders)
        : base(sessionReadyMessage.Sender.Name,
            sessionReadyMessage.Timestamp,
            localParticipant.Id == sessionReadyMessage.SenderId,
            isHostMessage,
            removeHeaders)
    {
        InitializeComponent();

        this.eventBus = eventBus;
        this.roomId = roomId;
        this.sessionReadyMessage = sessionReadyMessage;

        currentAmount = sessionReadyMessage.CurrentParticipantAmount;
        asDownloaded = sessionFeatureCollection.SessionScanner.DownloadedSessions.ContainsKey(sessionReadyMessage.Session.SessionId);

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        ApplyStylings();
        PerformLayout();
        SubscribeToEvents();
        FillLabels();
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<SessionRescanCompletedEvent>(OnSessionRescanCompletedEvent),
            eventBus.Subscribe<SessionParticipantJoinedEvent>(OnSessionParticipantJoined),
            eventBus.Subscribe<SessionParticipantLeftEvent>(OnSessionParticipantLeft)
        ];
    }

    private async Task OnSessionRescanCompletedEvent(SessionRescanCompletedEvent eventMessage, CancellationToken cancellationToken)
    {
        asDownloaded = eventMessage.DownloadedSessions.ContainsKey(sessionReadyMessage.Session.SessionId);

        uiContext.Post(_ =>
        {
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    private async Task OnSessionParticipantJoined(SessionParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId
            || eventMessage.SubRoomId != sessionReadyMessage.SubRoomId)
        {
            return;
        }

        currentAmount++;

        uiContext.Post(_ =>
        {
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    private async Task OnSessionParticipantLeft(SessionParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId
            || eventMessage.SubRoomId != sessionReadyMessage.SubRoomId)
        {
            return;
        }

        currentAmount--;

        uiContext.Post(_ =>
        {
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    ///<inheritdoc />
    public override void ApplyStylings()
    {
        if (removeHeaders)
        {
            Height -= Convert.ToInt32(TableLayoutPanel.RowStyles[0].Height);
        }
        base.ApplyStylings();

        sessionName.ForeColor = ColorScheme.TextPrimary;
        tableLayoutPanelLabels.BackColor = SenderColor;
        sessionName.Font = FontScheme.Heading3;
        joinButton.Font = FontScheme.Default;
    }

    private void FillLabels()
    {
        if (sessionReadyMessage.ThumbnailData != null)
        {
            using var ms = new MemoryStream(sessionReadyMessage.ThumbnailData);
            sessionImage.Image = Image.FromStream(ms);
        }
        else
        {
            sessionImage.Image = Resources.rocket;
        }
        UpdateStats();
    }

    private void UpdateStats()
    {
        sessionName.Text = $"{sessionReadyMessage.Session.Name} (v. {sessionReadyMessage.Session.Version})";

        var maximumParticipants = sessionReadyMessage.Session.MaximumParticipants;

        var participantsRatio = $"{currentAmount}"
            + (maximumParticipants.HasValue ? $"/{maximumParticipants.Value}" : string.Empty);

        var isFull = maximumParticipants.HasValue && currentAmount >= maximumParticipants.Value;

        joinButton.Text = !asDownloaded
            ? "Скачать сессию"
            : (isFull ? "Заполнено" : "Присоединиться") + $" (Учавствуют: {participantsRatio})";

        joinButton.Enabled = !maximumParticipants.HasValue || !isFull;

        if (currentAmount <= 0 || isFull)
        {
            sessionName.ForeColor = ColorScheme.TextOnAccent;
            tableLayoutPanelLabels.BackColor = ColorScheme.ConnectionDisabled;
            RecolorEntireCard(ColorScheme.ConnectionDisabled);
        }
        else
        {
            sessionName.ForeColor = ColorScheme.TextPrimary;
            tableLayoutPanelLabels.BackColor = SenderColor;
            RecolorEntireCard(SenderColor);
        }
    }

    private void joinButton_Click(object sender, EventArgs e)
    {
        if (asDownloaded)
        {
            OnJoinRequested?.Invoke();
            return;
        }

        if (MessageBox.Show($"Хотите скачать сессию {sessionReadyMessage.Session.Name}?\n" +
            $"Вы будете перенесены по ссылке {sessionReadyMessage.Session.DownloadLink}",
            "Скачивание сессии", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = sessionReadyMessage.Session.DownloadLink,
                UseShellExecute = true
            });
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    void IDisposable.Dispose()
    {
        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
    }
}
