using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Views.Forms.HelperForms;
using MIN.Discovery.Events;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Helpers.Contracts.Models;
using MIN.Helpers.Contracts.Models.Enums;
using MIN.Helpers.Services;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Панель обнаружения комнат
/// </summary>
public partial class DiscoveryPanelView : StyledPanelView
{
    private readonly IDiscoveryService discoveryService;
    private readonly IRoomConnector roomConnector;
    private readonly IRoomHoster roomHoster;
    private readonly IRoomStore roomStore;
    private readonly IRoomFactory roomFactory;
    private readonly IEventBus eventBus;
    private readonly ISettingsProvider settingsProvider;
    private readonly ILocalNetworkComputerProvider computerProvider;
    private readonly IIdentityService identityService;
    private readonly ParticipantInfo localParticipant;

    private Settings Settings => settingsProvider.GetSettings();
    private readonly SynchronizationContext uiContext;
    private readonly CancellationTokenSource cts;
    private CancellationTokenSource? discoveryCts;
    private bool isDiscovering;
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryPanelView"/>
    /// </summary>
    public DiscoveryPanelView(IDiscoveryService discoveryService,
        IRoomConnector roomConnector,
        IRoomHoster roomHoster,
        IRoomStore roomStore,
        IRoomFactory roomFactory,
        IEventBus eventBus,
        ISettingsProvider settingsProvider,
        ILocalNetworkComputerProvider computerProvider,
        IIdentityService identityService,
        ParticipantInfo localParticipant,
        SynchronizationContext uiContext,
        CancellationTokenSource cts)
    {
        InitializeComponent();
        ParseMachineName();

        this.discoveryService = discoveryService;
        this.roomConnector = roomConnector;
        this.roomHoster = roomHoster;
        this.roomStore = roomStore;
        this.roomFactory = roomFactory;
        this.eventBus = eventBus;
        this.settingsProvider = settingsProvider;
        this.localParticipant = localParticipant;
        this.computerProvider = computerProvider;
        this.uiContext = uiContext;
        this.cts = cts;

        SubscribeToEvents();

    }

    private void ParseMachineName()
    {
        if (CollegePCNameParser.TryParseComputerName(computerProvider.GetLocalMachineName(), out var roomNumber, out var _))
        {
            classNumber.Value = roomNumber;
        }
    }

    private void SubscribeToEvents()
    {
        eventBus.Subscribe<RoomDiscoveredEvent>(OnRoomDiscovered);
    }

    private async void discoverRooms_Click(object sender, EventArgs e)
    {
        if (isDiscovering)
        {
            discoveryCts?.Cancel();
            isDiscovering = false;
        }
        else
        {
            await PerformDiscovery();
        }
    }

    private async Task PerformDiscovery()
    {
        isDiscovering = true;

        var availablePCs = Settings.SearchMethod == SearchMethod.ClassRoom
                ? computerProvider.GetLocalNetworkMachineNames(classNumber.Value.ToString())
                : Settings.PreferredPCNames;

        uiContext.Post(_ =>
        {
            discoverRooms.Text = "Остановить поиск";
            splitContainerDiscoverRoom.Panel2Collapsed = false;
            discoveryProgressBar.Value = 1;
            discoveryProgressBar.Maximum = availablePCs.Count() + 1;
            flowLayoutPanelDiscoveredRooms.Controls.Clear();
            totalRoomsCount.Text = "Поиск комнат...";
        }, null);

        discoveryCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);

        using var subscriptionToken = eventBus.Subscribe((EndpointCheckedEvent _, CancellationToken _) =>
        {
            discoveryProgressBar.Value++;
            return Task.CompletedTask;
        });

        try
        {
            await discoveryService.DiscoverRoomsAsync(availablePCs,
                TimeSpan.FromMilliseconds(Settings.DiscoveryTimeout), discoveryCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Discovery failed: {ex.Message}", "Error");
        }
        finally
        {
            discoverRooms.Text = "Найти комнаты";
            splitContainerDiscoverRoom.Panel2Collapsed = true;
            var roomsCount = flowLayoutPanelDiscoveredRooms.Controls.Count;
            totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
            isDiscovering = false;
        }
    }

    private Task OnRoomDiscovered(RoomDiscoveredEvent e, CancellationToken cancellationToken)
    {
        uiContext.Post(_ =>
        {
            foreach (var discoveryInfo in e.RoomDiscoveryInfos)
            {
                var card = new RoomCard(eventBus, localParticipant,
                    discoveryInfo.Room, e.MachineName)
                {
                    Parent = flowLayoutPanelDiscoveredRooms
                };

                card.Clicked += () => OnRoomJoin(discoveryInfo.Room, discoveryInfo.Endpoint);
                card.Disposed += (s, _) =>
                {
                    totalRoomsCount.Text = $"Всего нашлось комнат: {flowLayoutPanelDiscoveredRooms.Controls.Count}";
                };
            }

            var roomsCount = flowLayoutPanelDiscoveredRooms.Controls.Count;
            totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
        }, null);

        return Task.CompletedTask;
    }

    private bool ResolveParticipant()
    {
        if (Settings.DefaultParticipantName != string.Empty)
        {
            localParticipant.Name = Settings.DefaultParticipantName;
            identityService.SetParticipant(localParticipant);
        }
        else
        {
            var participantCreateForm = new ParticipantCreateForm(identityService);
            if (participantCreateForm.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            Settings.DefaultParticipantName = identityService.SelfPartcipant.Name;
            settingsProvider.SaveSettings(Settings);
        }
        return true;
    }

    private async Task OnRoomJoin(RoomInfo roomInfo, IEndpoint endpoint)
    {
        if (roomConnector.IsConnected(roomInfo.Id))
        {
            MessageBox.Show($"Вы уже подключены к этой комнате", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;
        }

        ResolveParticipant();

        try
        {
            roomStore.Add(new Room(roomInfo));

            var connectionId = Guid.Empty;
            var connectCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);

            new LoadingForm(roomInfo.Id, eventBus, async room =>
            {
                if (room == null)
                {
                    return;
                }
                OpenChatForm(room, connectionId, isHost: false, endpoint);
                await eventBus.PublishAsync(new RoomJoinedEvent() { RoomId = room.Id });
            }, connectCts, DesktopConstants.RoomConnectionTimeoutMs).Show();

            roomFactory.GetOrCreateContext(roomInfo.Id)
                .Connections.RegisterLocalParticipant(localParticipant);

            connectionId = await roomConnector.ConnectAsync(roomInfo, endpoint,
                DesktopConstants.RoomConnectionTimeoutMs, connectCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Произошла ошибка: {ex.Message}");
        }
    }

    private void OpenChatForm(Room room, Guid connectionId, bool isHost, IEndpoint endpoint)
    {
        var chatForm = new ChatForm(
            chatService,
            eventBus,
            notificationService,
            logger,
            identityService,
            room,
            endpoint);

        chatForm.FormClosing += async (_, _) =>
        {
            await CleanUpAsync(room.Id, connectionId, isHost);
        };

        chatForm.Show();
    }

    private async Task CleanUpAsync(Guid roomId, Guid connectionId, bool isHost)
    {
        if (isHost)
        {
            await discoveryService.StopDiscoveryAsync(roomId);
            await roomHoster.StopHostingAsync(roomId);
        }
        else
        {
            await roomConnector.DisconnectAsync(roomId, connectionId);
        }

        await eventBus.PublishAsync(new RoomClosedEvent() { RoomId = roomId });
        roomFactory.DestroyContext(roomId);
        roomStore.Remove(roomId);
    }

    /// <inheritdoc />
    public override void ApplyStylings()
    {
        statusStrip.BackColor = ColorScheme.PrimaryAccent;
        totalRoomsCount.ForeColor = ColorScheme.TextOnAccent;
        discoveryProgressBar.ForeColor = ColorScheme.PrimaryAccent;
    }
}
