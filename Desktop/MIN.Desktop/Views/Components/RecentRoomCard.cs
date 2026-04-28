using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Stores.Contracts.Models;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Infrastructure.Events;

namespace MIN.Desktop.Components;

/// <summary>
/// Кнопка меню
/// </summary>
public partial class RecentRoomCard : UserControl, IDisposable
{
    private readonly IEventBus eventBus;
    private readonly RoomContext roomContext;
    private readonly RoomInfo roomInfo;
    private readonly SynchronizationContext uiContext;

    private HashSet<IDisposable> eventTokens = null!;
    private DateTime lastMessageReceivedAt = DateTime.Now;
    private string lastMessageContent = string.Empty;
    private int missedMessagesCount;
    private int currentAmount;
    private int maximumAmount;

    /// <summary>
    /// Событие по нажатию
    /// </summary>
    public event Action? Clicked;

    /// <summary>
    /// Имя комнаты
    /// </summary>
    public string RoomName { get; private set; }

    /// <summary>
    /// Выбрана ли карточка
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomDiscoveryCard"/>
    /// </summary>
    /// <remarks>
    /// Room нужно получить по ссылке из store
    /// </remarks>
    public RecentRoomCard(IEventBus eventBus, RoomContext roomContext, RoomInfo roomInfo)
    {
        InitializeComponent();
        this.eventBus = eventBus;
        this.roomContext = roomContext;
        this.roomInfo = roomInfo;

        RoomName = roomInfo.Name;
        currentAmount = roomInfo.ParticipantCount + 1;
        maximumAmount = roomInfo.MaximumParticipants;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        GetLastMessage();
        ApplyStylings();
        UpdateStats();
        SubscribeToEvents();
    }

    private void GetLastMessage()
    {
        var lastMessage = roomContext.Messages.GetLastMessage();

        switch (lastMessage)
        {
            case ChatTextMessage chatTextMessage:
                lastMessageContent = $"{chatTextMessage.Sender.Name}: {chatTextMessage.Content}";
                return;

            case ParticipantJoinedMessage participantJoinedMessage:
                lastMessageContent = $"Участник {participantJoinedMessage.Participant.Name} зашёл в комнату";
                return;

            case ParticipantLeftMessage participantLeftMessage:
                lastMessageContent = $"Участник {participantLeftMessage.Participant.Name} покинул комнату";
                return;

            case SystemTextMessage systemTextMessage:
                lastMessageContent = systemTextMessage.Content;
                return;
        }
    }

    /// <summary>
    /// Выбрать карточку
    /// </summary>
    public void SelectCard()
    {
        IsSelected = true;
        missedMessagesCountLabel.Visible = !IsSelected;
        missedMessagesCount = 0;
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined),
            eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft),
            eventBus.Subscribe<RoomInfoChangedEvent>(OnRoomInfoChangedEvent),
            eventBus.Subscribe<ChatMessageReceivedEvent>(OnChatMessageReceivedEvent),
            eventBus.Subscribe<RoomClosedEvent>(OnRoomLeft),
        ];
    }

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Message.RoomId != roomInfo.Id)
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

    private async Task OnParticipantLeft(ParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Message.RoomId != roomInfo.Id)
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

    private Task OnRoomLeft(RoomClosedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomInfo.Id)
        {
            return Task.CompletedTask;
        }

        Dispose();
        return Task.CompletedTask;
    }

    private Task OnChatMessageReceivedEvent(ChatMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomInfo.Id)
        {
            return Task.CompletedTask;
        }

        uiContext.Post(_ =>
        {
            if (!IsSelected)
            {
                missedMessagesCount++;
            }
            lastMessageReceivedAt = DateTime.Now;
            lastMessageContent = eventMessage.Sender == null
                ? eventMessage.Content
                : $"{eventMessage.Sender}: {eventMessage.Content}";
            UpdateStats();
        }, null);

        return Task.CompletedTask;
    }

    private async Task OnRoomInfoChangedEvent(RoomInfoChangedEvent eventMessage, CancellationToken ct)
    {
        if (eventMessage.Room.Id != roomInfo.Id)
        {
            return;
        }

        RoomName = eventMessage.Room.Name;
        roomInfo.Name = eventMessage.Room.Name;
        maximumAmount = eventMessage.Room.MaximumParticipants;

        uiContext.Post(_ =>
        {
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    private void ApplyStylings()
    {
        missedMessagesCountLabel.BackColor = ColorScheme.PrimaryAccent;
        missedMessagesCountLabel.ForeColor = ColorScheme.TextOnAccent;
        BackColor = ColorScheme.Transparent;
    }

    private void UpdateStats()
    {
        Title.Text = $"Комната {roomInfo.Name}";
        participantsInfo.Text = $"{currentAmount}/{maximumAmount}";
        lastMessageTime.Text = lastMessageReceivedAt.ToShortTimeString();
        lastMessageSenderAndContent.Text = lastMessageContent;
        missedMessagesCountLabel.Text = missedMessagesCount.ToString();
        missedMessagesCountLabel.Visible = !IsSelected || missedMessagesCount > 0;
    }

    private void RecentRoomCard_Click(object sender, EventArgs e)
    {
        Clicked?.Invoke();
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
