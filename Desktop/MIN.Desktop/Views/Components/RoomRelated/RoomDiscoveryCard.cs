using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Infrastructure.Services;

namespace MIN.Desktop.Components;

/// <summary>
/// Кнопка меню
/// </summary>
public partial class RoomDiscoveryCard : UserControl, IDisposable
{
    private readonly IEventBus eventBus;
    private readonly RoomInfo room;
    private readonly SynchronizationContext uiContext;
    private readonly bool isOwner;

    private HashSet<IDisposable> eventTokens = null!;

    /// <summary>
    /// Событие по нажатию
    /// </summary>
    public event Func<Task>? Clicked;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomDiscoveryCard"/>
    /// </summary>
    public RoomDiscoveryCard(IEventBus eventBus, ParticipantInfo localParticipant, RoomInfo room)
    {
        InitializeComponent();
        this.eventBus = eventBus;
        this.room = room;
        isOwner = room.HostParticipant.Id == localParticipant.Id;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        ApplyStylings();
        UpdateStats();
        SubscribeToEvents();
    }

    /// <summary>
    /// Включить кнопку подключения обратно
    /// </summary>
    public void EnableConnectButton()
    {
        connectButton.Enabled = true;
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined),
            eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft),
            eventBus.Subscribe<RoomInfoUpdatedMessageEvent>(OnRoomInfoUpdatedMessageEvent),
            eventBus.Subscribe<RoomClosedEvent>(OnRoomLeft),
            eventBus.Subscribe<RoomJoinedEvent>(OnRoomJoined),
            eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccured),
        ];
    }

    private async Task OnErrorOccured(ErrorOccurredEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != eventMessage.RoomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            connectButton.Enabled = true;
        }, this);

        await Task.CompletedTask;
    }

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
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

    private async Task OnParticipantLeft(ParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != room.Id)
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
            Dispose();
            return;
        }
        room.ParticipantCount--;

        uiContext.Post(_ =>
        {
            UpdateStats();
        }, null);

        await Task.CompletedTask;
    }

    private async Task OnRoomJoined(RoomJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != room.Id)
        {
            return;
        }

        connectButton.Enabled = true;
        room.Name = eventMessage.RoomInfo.Name;
        room.MaximumParticipants = eventMessage.RoomInfo.MaximumParticipants;
        room.ParticipantCount = eventMessage.RoomInfo.ParticipantCount;

        uiContext.Post(_ =>
        {
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    private async Task OnRoomInfoUpdatedMessageEvent(RoomInfoUpdatedMessageEvent eventMessage, CancellationToken ct)
    {
        if (eventMessage.RoomInfo.Id != room.Id)
        {
            return;
        }

        room.Name = eventMessage.RoomInfo.Name;
        room.MaximumParticipants = eventMessage.RoomInfo.MaximumParticipants;
        room.ParticipantCount = eventMessage.RoomInfo.ParticipantCount;

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
        computer.Text = room.PcNumber.ToString();

        if (IpAddressParser.TryParseIpAddress(room.ConnectionAddress, out var gottenIpAddress, out var port))
        {
            connectionPort.Text = port.ToString();
            connectionAddress.Text = gottenIpAddress;
        }

        classroom.Text = string.IsNullOrEmpty(room.Cabinet)
            ? DesktopConstants.UndefinedPcName
            : room.Cabinet;

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
        connectButton.Enabled = false;
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
