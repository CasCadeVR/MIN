using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Contracts.Models.Enums;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Views.Panels.SidePanelViews;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop;

/// <summary>
/// Главная форма приложения
/// </summary>
public partial class MainForm : StyledForm, INavigationService
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

    private BasePanelView currentPanelView = null!;

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
        InitializeViews();

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
    }

    private void InitializeViews()
    {
        NavigateCoreTo(new NavigationItem()
        {
            PanelType = PanelType.Side,
            ViewInstance = new MainSidePanelView()
        });
        NavigateCoreTo(new NavigationItem()
        {
            PanelType = PanelType.Main,
            ViewInstance = new DiscoveryPanelView(discoveryService, roomConnector, roomHoster,
            roomStore, roomFactory, eventBus, settingsProvider, computerProvider,
                localParticipant, uiContext, cts)
        });
    }

    void INavigationService.NavigateTo(NavigationItem item)
    {
        NavigateCoreTo(item);
    }

    private void NavigateCoreTo(NavigationItem item)
    {
        if (currentPanelView != null)
        {
            currentPanelView.RequestNavigate -= NavigateCoreTo;
        }

        Panel container = null!;

        switch (item.PanelType)
        {
            case PanelType.Main:
                container = mainPanel;
                break;

            case PanelType.Side:
                container = sidePanel;
                break;
        }

        container.Controls.Clear();

        BasePanelView view;

        if (item.ViewInstance != null)
        {
            view = item.ViewInstance;
        }
        else
        {
            throw new InvalidOperationException("Ошибка: не было установлено значение у навигационной модели");
        }

        view.RequestNavigate += NavigateCoreTo;
        currentPanelView = view;

        view.OnNavigation(item);
        view.Dock = DockStyle.Fill;

        container.Controls.Add(view);
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        sidePanel.BackColor = ColorScheme.MainPanelBackground;
        mainPanel.BackColor = ColorScheme.FormBackground;
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

    private void MainForm_Load(object sender, EventArgs e)
    {
        localParticipant = new ParticipantInfo()
        {
            Name = "Ты",
        };

        identityService.SetParticipant(localParticipant);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        cts.Cancel();
        cts.Dispose();
    }
}
