using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Infrastructure.Events;
using MIN.Helpers.Services;

namespace MIN.Desktop.Components;

/// <summary>
/// Кнопка меню
/// </summary>
public partial class RoomCard : UserControl, IDisposable
{
    private readonly IEventBus eventBus;
    private readonly RoomInfo room;
    private readonly string computerName;
    private readonly SynchronizationContext uiContext;
    private readonly bool isOwner;

    private HashSet<IDisposable> eventTokens = null!;

    /// <summary>
    /// Событие по нажатию
    /// </summary>
    public event Func<Task>? Clicked;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomCard"/>
    /// </summary>
    public RoomCard(IEventBus eventBus, ParticipantInfo localParticipant, RoomInfo room, string computerName)
    {
        InitializeComponent();
        this.eventBus = eventBus;
        this.room = room;
        this.computerName = computerName;
        isOwner = room.HostParticipant.Id == localParticipant.Id;

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
            eventBus.Subscribe<RoomJoinedEvent>(OnRoomJoined),
        ];
    }

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Message.RoomId != room.Id)
        {
            return;
        }

        room.ParticipantCount++;

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

        room.ParticipantCount--;

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

        if (isOwner)
        {
            uiContext.Post(_ =>
            {
                Dispose();
            }, null);
        }
        else
        {
            room.ParticipantCount--;

            uiContext.Post(_ =>
            {
                UpdateStats();
            }, null);
        }

        await Task.CompletedTask;
    }

    private async Task OnRoomJoined(RoomJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != room.Id)
        {
            return;
        }

        room.ParticipantCount++;

        uiContext.Post(_ =>
        {
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    private void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        tableLayoutPanelLabels.BackColor = ColorScheme.MainPanelBackground;
        Title.ForeColor = ColorScheme.TextOnAccent;
    }

    private void UpdateStats()
    {
        Title.Text = $"Комната {room.Name}";
        participantsInfo.Text = $"{room.ParticipantCount}/{room.MaximumParticipants}";
        hostName.Text = room.HostParticipant.Name;
        createdAt.Text = room.CreatedAt.ToShortTimeString();

        if (CollegePCNameParser.TryParseComputerName(computerName, out var roomNumber, out var computerNumber))
        {
            computer.Text = computerNumber.ToString();
            classroom.Text = roomNumber.ToString();
        }
        else
        {
            computer.Text = DesktopConstants.UndefinedPCName;
            classroom.Text = DesktopConstants.UndefinedPCName;
        }

        ManageConnectButtonAccessability();
    }

    private void ManageConnectButtonAccessability()
    {
        var isFull = room.ParticipantCount >= room.MaximumParticipants;
        var isNotAccessible = isFull || isOwner;

        connectButton.Enabled = !isNotAccessible;

        if (isNotAccessible)
        {
            connectButton.Text = isFull ? "Заполнено" : "Твоя комната";
            connectButton.BackColor = ColorScheme.ConnectionDisabled;
        }
        else
        {
            connectButton.Text = "Присоединиться";
            connectButton.BackColor = ColorScheme.SecondaryAccent;
        }
    }

    private void connectButton_Click(object sender, EventArgs e)
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
