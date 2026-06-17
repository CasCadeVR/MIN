using MIN.Chat.Events;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts;
using MIN.Desktop.Components.Controls.ContextMenuStrips;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка участника в комнате
/// </summary>
public partial class ParticipantCard : UserControl, IDisposable
{
    private const string StartPrivateChatText = "Начать приватное общение";
    private const string StopPrivateChatText = "Прекратить приватное общение";

    private readonly Participant participant;
    private readonly IEventBus eventBus;
    private readonly Guid roomId;
    private readonly SynchronizationContext uiContext;
    private readonly bool isHost;
    private readonly bool isSelf;
    private HashSet<IDisposable> eventTokens = null!;
    private bool selected;

    /// <summary>
    /// Идентфикатор участника на карточке
    /// </summary>
    public Guid ParticipantId => participant.Id;

    /// <summary>
    /// Событие по нажатию на кнопку начала приватного общения у участника
    /// </summary>
    public Action<bool, Participant>? OnPrivateChatMenuStripClicked { get; set; }

    /// <summary>
    /// Событие по нажатию на кнопку кика участника
    /// </summary>
    public Action<Participant>? OnKickParticipantClicked { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantCard"/>
    /// </summary>
    public ParticipantCard(Participant participant,
        IEventBus eventBus,
        Guid roomId,
        bool isHost,
        bool isSelf,
        bool asHost)
    {
        InitializeComponent();
        ApplyStylings();
        this.participant = participant;
        this.eventBus = eventBus;
        this.roomId = roomId;
        this.isHost = isHost;
        this.isSelf = isSelf;

        FillLabels();

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        if (!isSelf)
        {
            var participantCardContextMenuStrip = new ParticipantCardContextMenuStrip();
            participantCardContextMenuStrip.OnPrivateChatClick += OnPrivateChatClickMenuStripClicked;
            participantCardContextMenuStrip.Items[0].Text = StartPrivateChatText;
            participantCardContextMenuStrip.OnKickClick += OnKickParticipantClickMenuStripClicked;
            participantCardContextMenuStrip.Items[1].Visible = asHost;
            ContextMenuStrip = participantCardContextMenuStrip;
        }
        if (!isSelf)
        {
            SubscribeToEvents();
        }
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<OnlineStatusChangedEvent>(OnOnlineStatusChanged),
        ];
    }

    private async Task OnOnlineStatusChanged(OnlineStatusChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            if (eventMessage.SenderId == participant.Id)
            {
                UpdateStatus(eventMessage.Status);
            }
        }, null);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Отменить выбор карточки
    /// </summary>
    public void Unselect()
    {
        selected = false;
        UpdateStylesOutOfSelected();
    }

    private void OnPrivateChatClickMenuStripClicked()
    {
        selected = !selected;
        UpdateStylesOutOfSelected();
        OnPrivateChatMenuStripClicked?.Invoke(selected, participant);
    }

    private void OnKickParticipantClickMenuStripClicked()
    {
        OnKickParticipantClicked?.Invoke(participant);
    }

    private void UpdateStylesOutOfSelected()
    {
        if (ContextMenuStrip != null)
        {
            ContextMenuStrip.Items[0].Text = selected ? StopPrivateChatText : StartPrivateChatText;
        }
        BackColor = selected
            ? ColorScheme.PrivateParticipantCardBackground
            : ColorScheme.DefaultParticipantCardBackground;
    }

    private void ApplyStylings()
    {
        participantName.Font = FontScheme.Caption;
        currentStatus.Font = FontScheme.MicroCaption;
        participantRole.Font = FontScheme.Caption;
        BackColor = ColorScheme.DefaultParticipantCardBackground;
    }

    private void UpdateStatus(OnlineStatus status)
    {
        var resultText = string.Empty;
        switch (status)
        {
            case OnlineStatus.Online:
                resultText = "В сети";
                break;

            case OnlineStatus.Typing:
                resultText = "Печатает . . .";
                break;

            case OnlineStatus.Offline:
                resultText = $"Последний раз в сети: {participant.LastSeenOnline:t}";
                break;
        }
        currentStatus.Text = resultText;
    }

    private void FillLabels()
    {
        UpdateStatus(isSelf ? OnlineStatus.Online : participant.CurrentStatus);
        participantName.Text = participant.Name;
        if (isHost)
        {
            participantRole.Text = "Хост";
        }
        else
        {
            participantRole.Text = "";
            tableLayoutPanelLabels.ColumnStyles[1].Width = 0;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    void IDisposable.Dispose()
    {
        if (eventTokens == null)
        {
            return;
        }

        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
    }
}
