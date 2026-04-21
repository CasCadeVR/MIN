using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Views.Forms.HelperForms;
using MIN.Discovery.Events;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Helpers.Contracts.Models;
using MIN.Helpers.Contracts.Models.Enums;
using MIN.Helpers.Services;

namespace MIN.Desktop;

/// <summary>
/// Главная форма приложения
/// </summary>
public partial class MainForm : StyledForm
{
    private readonly IRoomConnector roomConnector;
    private readonly IRoomHoster roomHoster;
    private readonly IRoomStore roomStore;
    private readonly IRoomFactory roomFactory;
    private readonly IChatService chatService;
    private readonly IDiscoveryService discoveryService;
    private readonly IEventBus eventBus;
    private readonly ISettingsProvider settingsProvider;
    private readonly INotificationService notificationService;
    private readonly ILocalNetworkComputerProvider computerProvider;
    private readonly ILoggerProvider logger;
    private readonly IIdentityService identityService;

    private readonly Version version;
    private readonly SynchronizationContext uiContext;
    private readonly CancellationTokenSource cts;

    private Settings Settings => settingsProvider.GetSettings();
    private ParticipantInfo localParticipant = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainForm"/>
    /// </summary>
    public MainForm(
        IRoomConnector roomConnector,
        IRoomHoster roomHoster,
        IRoomStore roomStore,
        IRoomFactory roomFactory,
        IChatService chatService,
        IDiscoveryService discoveryService,
        IEventBus eventBus,
        ISettingsProvider settingsProvider,
        INotificationService notificationService,
        ILocalNetworkComputerProvider computerProvider,
        IIdentityService identityService,
        ILoggerProvider logger,
        Version version)
    {
        InitializeComponent();

        this.roomConnector = roomConnector;
        this.roomHoster = roomHoster;
        this.roomStore = roomStore;
        this.roomFactory = roomFactory;
        this.chatService = chatService;
        this.discoveryService = discoveryService;
        this.eventBus = eventBus;
        this.settingsProvider = settingsProvider;
        this.notificationService = notificationService;
        this.computerProvider = computerProvider;
        this.identityService = identityService;
        this.logger = logger;
        this.version = version;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        cts = new CancellationTokenSource();

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        eventBus.Subscribe<RoomDiscoveredEvent>(OnRoomDiscovered);
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainerClass.Panel1.BackColor = ColorScheme.MainPanelBackground;
        splitContainerClass.Panel2.BackColor = ColorScheme.FormBackground;
        statusStrip.BackColor = ColorScheme.PrimaryAccent;
        totalRoomsCount.ForeColor = ColorScheme.TextOnAccent;
        discoveryProgressBar.ForeColor = ColorScheme.PrimaryAccent;
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

    private async void createRoom_Click(object sender, EventArgs e)
    {
        var roomCreateForm = new RoomCreateForm();

        if (roomCreateForm.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        var room = roomCreateForm.Room;

        if (!ResolveParticipant())
        {
            return;
        }

        localParticipant = identityService.SelfPartcipant.ToParticipantInfo();
        var context = roomFactory.GetOrCreateContext(room.Id);

        context.Connections.RegisterLocalParticipant(localParticipant);
        room.HostParticipant = localParticipant;

        try
        {
            roomStore.Add(room);

            var roomInfo = new RoomInfo(room);
            await roomHoster.StartHostingAsync(roomInfo, cts.Token);

            context.Participants.AddParticipant(localParticipant);

            await discoveryService.StartDiscoveryAsync(roomInfo.Id, cts.Token);

            OpenChatForm(roomStore.GetRoom(room.Id),
                CoreRegistryConstants.LocalConnectionId,
                isHost: true, new NamedPipeEndpoint()
                {
                    MachineName = Environment.MachineName,
                });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Создание комнаты прошло не успешно: {ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
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

    private async void discoverRooms_Click(object sender, EventArgs e)
    {
        await PerformDiscovery();
    }

    private async Task PerformDiscovery()
    {
        var availablePCs = Settings.SearchMethod == SearchMethod.ClassRoom
                ? computerProvider.GetLocalNetworkMachineNames(classNumber.Value.ToString())
                : Settings.PreferredPCNames;

        uiContext.Post(_ =>
        {
            discoverRooms.Enabled = false;
            splitContainerDiscovery.Panel1Collapsed = true;
            splitContainerDiscovery.Panel2Collapsed = false;
            discoveryProgressBar.Value = 1;
            discoveryProgressBar.Maximum = availablePCs.Count() + 1;
            flowLayoutPanel.Controls.Clear();
            totalRoomsCount.Text = "Поиск комнат...";
        }, null);

        using var subscriptionToken = eventBus.Subscribe((EndpointCheckedEvent _, CancellationToken _) =>
            {
                discoveryProgressBar.Value++;
                return Task.CompletedTask;
            });

        try
        {
            await discoveryService.DiscoverRoomsAsync(availablePCs, TimeSpan.FromMilliseconds(Settings.DiscoveryTimeout), cts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Discovery failed: {ex.Message}", "Error");
        }
        finally
        {
            discoverRooms.Enabled = true;
            splitContainerDiscovery.Panel1Collapsed = false;
            splitContainerDiscovery.Panel2Collapsed = true;
            var roomsCount = flowLayoutPanel.Controls.Count;
            totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
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
                    Parent = flowLayoutPanel
                };

                card.Clicked += () => OnRoomJoin(discoveryInfo.Room, discoveryInfo.Endpoint);
                card.Disposed += (s, _) =>
                {
                    totalRoomsCount.Text = $"Всего нашлось комнат: {flowLayoutPanel.Controls.Count}";
                };
            }

            var roomsCount = flowLayoutPanel.Controls.Count;
            totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
        }, null);

        return Task.CompletedTask;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        localParticipant = new ParticipantInfo()
        {
            Name = "Ты",
        };

        identityService.SetParticipant(localParticipant);

        if (CollegePCNameParser.TryParseComputerName(Environment.MachineName, out var roomNumber, out var _))
        {
            classNumber.Value = roomNumber;
        }
    }

    private void settingsButton_Click(object sender, EventArgs e)
    {
        var settingsForm = new SettingsForm(Settings, version, logger);
        if (settingsForm.ShowDialog() == DialogResult.OK)
        {
            settingsProvider.SaveSettings(settingsForm.Settings);
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        cts.Cancel();
        cts.Dispose();
    }
}
