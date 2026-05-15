using MIN.Chat.Events;
using MIN.Chat.Services.Contracts.Models.Enums;
using MIN.Core.Entities.Contracts.Models;
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

    private readonly ParticipantInfo participant;
    private readonly IEventBus eventBus;
    private readonly Guid roomId;
    private readonly SynchronizationContext uiContext;
    private readonly bool isHost;
    private HashSet<IDisposable> eventTokens = null!;
    private DateTime lastOnline;
    private bool selected;

    /// <summary>
    /// Событие по нажатию на контекстное меню карточки
    /// </summary>
    public Action<bool, ParticipantInfo>? OnCardContextMenuStripClicked { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantCard"/>
    /// </summary>
    public ParticipantCard(ParticipantInfo participant,
        IEventBus eventBus,
        Guid roomId,
        bool isHost,
        bool isSelf)
    {
        InitializeComponent();
        ApplyStylings();
        this.participant = participant;
        this.eventBus = eventBus;
        this.roomId = roomId;
        this.isHost = isHost;
        FillLabels();

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        if (!isSelf)
        {
            var pictureBoxContextMenuStrip = new ParticipantCardContextMenuStrip();
            pictureBoxContextMenuStrip.OnItemClick += CardContextMenuStripClicked;
            pictureBoxContextMenuStrip.Items[0].Text = StartPrivateChatText;
            ContextMenuStrip = pictureBoxContextMenuStrip;
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
                if (eventMessage.Status == OnlineStatus.Offline)
                {
                    lastOnline = DateTime.Now;
                }
                UpdateStatus(eventMessage.Status);
            }
        }, null);
        await Task.CompletedTask;
    }

    private void CardContextMenuStripClicked()
    {
        selected = !selected;
        ContextMenuStrip!.Items[0].Text = selected ? StopPrivateChatText : StartPrivateChatText;
        BackColor = selected
            ? ColorScheme.PrivateParticipantCardBackground
            : ColorScheme.DefaultParticipantCardBackground;
        OnCardContextMenuStripClicked?.Invoke(selected, participant);
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
                resultText = $"Последний раз в сети: {lastOnline:t}";
                break;
        }
        currentStatus.Text = resultText;
    }

    private void FillLabels()
    {
        UpdateStatus(OnlineStatus.Online);
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
