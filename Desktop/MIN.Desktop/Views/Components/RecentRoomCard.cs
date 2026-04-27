using MIN.Chat.Messaging;
using MIN.Core.Entities;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated;
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
    private readonly Room room;
    private readonly SynchronizationContext uiContext;

    private HashSet<IDisposable> eventTokens = null!;
    private int currentAmount;
    private int maximumAmount;

    /// <summary>
    /// Событие по нажатию
    /// </summary>
    public event Action? Clicked;

    /// <summary>
    /// Имя комнаты
    /// </summary>
    public string RoomName { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomDiscoveryCard"/>
    /// </summary>
    public RecentRoomCard(IEventBus eventBus, RoomContext roomContext, Room room)
    {
        InitializeComponent();
        this.eventBus = eventBus;
        this.roomContext = roomContext;
        this.room = room;
        RoomName = room.Name;
        currentAmount = room.CurrentParticipants.Count;
        maximumAmount = room.MaximumParticipants;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        ApplyStylings();
        UpdateStats();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined),
            eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft),
            eventBus.Subscribe<RoomClosedEvent>(OnRoomLeft),
        ];
    }

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Message.RoomId != room.Id)
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
        if (eventMessage.Message.RoomId != room.Id)
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

    private async Task OnRoomLeft(RoomClosedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != room.Id)
        {
            return;
        }

        Dispose();
        await Task.CompletedTask;
    }

    private void ApplyStylings()
    {
        BackColor = ColorScheme.Transparent;
    }

    private void UpdateStats()
    {
        Title.Text = $"Комната {room.Name}";
        participantsInfo.Text = $"{currentAmount}/{maximumAmount}";

        var lastMessage = room.ChatHistory.LastOrDefault();

        if (lastMessage == null)
        {
            lastMessageTime.Text = string.Empty;
            lastMessageSenderAndContent.Text = string.Empty;
            return;
        }

        lastMessageTime.Text = lastMessage.Timestamp.ToShortTimeString();

        roomContext.Participants.TryGetParticipantById(lastMessage.SenderId, out var senderInfo);

        lastMessageSenderAndContent.Text = senderInfo?.Name ?? string.Empty;

        switch (lastMessage)
        {
            case ChatTextMessage chatTextMessage:
                lastMessageSenderAndContent.Text = $"{chatTextMessage.Sender.Name}: {chatTextMessage.Content}";
                break;

            case SystemTextMessage systemTextMessage:
                lastMessageSenderAndContent.Text = systemTextMessage.Content;
                break;
        }
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
