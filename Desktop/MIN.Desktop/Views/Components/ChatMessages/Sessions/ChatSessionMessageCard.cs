using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Components.ChatMessages;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения, представляющая файл от пользователя
/// </summary>
public partial class ChatSessionMessageCard : BaseChatMessageCard, IDisposable
{
    private readonly IEventBus eventBus = null!;
    private readonly SessionReadyMessage sessionReadyMessage = null!;
    private readonly SynchronizationContext uiContext = null!;
    private HashSet<IDisposable> eventTokens = null!;
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
    public ChatSessionMessageCard(IEventBus eventBus,
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
        this.sessionReadyMessage = sessionReadyMessage;

        currentAmount = sessionReadyMessage.CurrentParticipantAmount;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        FillLabels();
        ApplyStylings();
        PerformLayout();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<SessionParticipantJoinedEvent>(OnSessionParticipantJoined),
            eventBus.Subscribe<SessionParticipantLeftEvent>(OnSessionParticipantLeft)
        ];
    }

    private async Task OnSessionParticipantJoined(SessionParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != sessionReadyMessage.RoomId
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
        if (eventMessage.RoomId != sessionReadyMessage.RoomId
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

        tableLayoutPanelLabels.BackColor = SenderColor;
        sessionName.Font = FontScheme.Heading3;
        joinButton.Font = FontScheme.Default;
    }

    private void FillLabels()
    {
        sessionName.Text = sessionReadyMessage.Session.Name;
        sessionImage.Image = SessionImageProvider.LoadImageOutOfSessionType(sessionReadyMessage.Session.SessionType);
        UpdateStats();
    }

    private void UpdateStats()
    {
        joinButton.Text = $"Присоединиться (Учавствуют: {currentAmount})";
    }

    private void joinButton_Click(object sender, EventArgs e)
    {
        OnJoinRequested?.Invoke();
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
