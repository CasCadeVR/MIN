using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Services.Contracts.Constants;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Interfaces.Stores;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Discovery.Events;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models;
using MIN.Helpers.Contracts.Models.Enums;
using MIN.Helpers.Services;

namespace MIN.Desktop
{
    /// <summary>
    /// Главная форма приложения
    /// </summary>
    public partial class MainForm : StyledForm
    {
        private readonly IRoomConnector roomConnector;
        private readonly IRoomHoster roomHoster;
        private readonly IRoomStore roomStore;
        private readonly IParticipantConnectionRegistry participantConnectionRegistry;
        private readonly IParticipantStore participantStore;
        private readonly IChatService chatService;
        private readonly IDiscoveryService discoveryService;
        private readonly IEventBus eventBus;
        private readonly ISettingsProvider settingsProvider;
        private readonly INotificationService notificationService;
        private readonly ILocalNetworkComputerProvider computerProvider;
        private readonly ILoggerProvider logger;
        private readonly IIdentityService identityService;

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
            IParticipantConnectionRegistry participantConnectionRegistry,
            IParticipantStore participantStore,
            IChatService chatService,
            IDiscoveryService discoveryService,
            IEventBus eventBus,
            ISettingsProvider settingsProvider,
            INotificationService notificationService,
            ILocalNetworkComputerProvider computerProvider,
            IIdentityService identityService,
            ILoggerProvider logger)
        {
            InitializeComponent();

            this.roomConnector = roomConnector;
            this.roomHoster = roomHoster;
            this.roomStore = roomStore;
            this.participantConnectionRegistry = participantConnectionRegistry;
            this.participantStore = participantStore;
            this.chatService = chatService;
            this.discoveryService = discoveryService;
            this.eventBus = eventBus;
            this.settingsProvider = settingsProvider;
            this.notificationService = notificationService;
            this.computerProvider = computerProvider;
            this.identityService = identityService;
            this.logger = logger;

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
        }

        private async void createRoom_Click(object sender, EventArgs e)
        {
            var roomCreateForm = new RoomCreateForm();

            if (roomCreateForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var room = roomCreateForm.Room;

            var participantCreateForm = new ParticipantCreateForm(identityService);
            if (participantCreateForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            localParticipant = new ParticipantInfo(identityService.SelfPartcipant);
            room.HostParticipant = localParticipant;

            try
            {
                roomStore.Add(room);

                var roomInfo = new RoomInfo(room);
                await roomHoster.StartHostingAsync(roomInfo, cts.Token);

                participantStore.AddParticipant(roomInfo.Id, localParticipant);

                await discoveryService.StartDiscoveryAsync(roomInfo.Id, cts.Token);

                OpenChatForm(roomInfo.Id, CoreServicesConstants.LocalConnectionId, isHost: true, new NamedPipeEndpoint()
                {
                    MachineName = Environment.MachineName,
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Создание комнаты прошло не успешно: {ex.Message}", "Ошибка");
            }
        }

        private async Task OnRoomJoin(RoomInfo roomInfo, IEndpoint endpoint)
        {
            if (roomInfo.ParticipantCount >= roomInfo.MaximumParticipants)
            {
                MessageBox.Show("Невозможно подключиться: комната заполнена.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var participantCreateForm = new ParticipantCreateForm(identityService);
            if (participantCreateForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                roomStore.Add(new Room(roomInfo));
                var connectionId = await roomConnector.ConnectAsync(roomInfo,
                    endpoint,
                    Settings.DiscoveryTimeout,
                    cts.Token);

                OpenChatForm(roomInfo.Id, connectionId, isHost: false, endpoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void OpenChatForm(Guid roomId, Guid connectionId, bool isHost, IEndpoint endpoint)
        {
            var chatForm = new ChatForm(
                chatService,
                roomStore,
                eventBus,
                notificationService,
                logger,
                identityService,
                roomId,
                endpoint);

            chatForm.FormClosing += async (_, _) =>
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
            };

            chatForm.Show();
        }

        private async Task PerformSearch()
        {
            uiContext.Post(_ =>
            {
                findRooms.Enabled = false;
            }, null);

            try
            {
                var availablePCs = Settings.SearchMethod == SearchMethod.ClassRoom
                    ? computerProvider.GetLocalNetworkMachineNames(classNumber.Value.ToString())
                    : Settings.PreferredPCNames;

                flowLayoutPanel.Controls.Clear();
                totalRoomsCount.Text = "Поиск комнат...";

                await discoveryService.DiscoverRoomsAsync(availablePCs, TimeSpan.FromMilliseconds(Settings.DiscoveryTimeout), cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Discovery failed: {ex.Message}", "Error");
            }
            finally
            {
                uiContext.Post(_ =>
                {
                    findRooms.Enabled = true;

                    var roomsCount = flowLayoutPanel.Controls.Count;
                    totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
                }, null);
            }
        }

        private Task OnRoomDiscovered(RoomDiscoveredEvent e, CancellationToken cancellationToken)
        {
            uiContext.Post(_ =>
            {
                foreach (var discoveryInfo in e.RoomDiscoveryInfos)
                {
                    var card = new RoomCard(localParticipant, discoveryInfo.Room)
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

        private async void findRooms_Click(object sender, EventArgs e)
        {
            await PerformSearch();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            localParticipant = new ParticipantInfo()
            {
                Name = "Ты",
            };

            identityService.SetParticipant(localParticipant);
            participantConnectionRegistry.RegisterLocalParticipant(localParticipant);

            if (CollegePCNameParser.TryParseComputerName(Environment.MachineName, out var roomNumber, out var _))
            {
                classNumber.Value = roomNumber;
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(Settings, logger);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                settingsProvider.SaveSettings(settingsForm.Settings);
            }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            await roomHoster.StopHostingAsync(Guid.Empty);
            await roomConnector.DisconnectAsync(Guid.Empty, Guid.Empty);
            cts.Cancel();
            cts.Dispose();
        }
    }
}
