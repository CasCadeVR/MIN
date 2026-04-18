using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Discovery.Events;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
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
        private readonly IParticipantStore participantStore;
        private readonly IMessageStore messageStore;
        private readonly IParticipantConnectionRegistry participantConnectionRegistry;
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
            IParticipantStore participantStore,
            IMessageStore messageStore,
            IParticipantConnectionRegistry participantConnectionRegistry,
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
            this.participantStore = participantStore;
            this.messageStore = messageStore;
            this.participantConnectionRegistry = participantConnectionRegistry;
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

            localParticipant = identityService.SelfPartcipant.ToParticipantInfo();
            participantConnectionRegistry.RegisterLocalParticipant(room.Id, localParticipant);
            room.HostParticipant = localParticipant;

            try
            {
                roomStore.Add(room);

                var roomInfo = new RoomInfo(room);
                await roomHoster.StartHostingAsync(roomInfo, cts.Token);

                participantStore.AddParticipant(roomInfo.Id, localParticipant);

                await discoveryService.StartDiscoveryAsync(roomInfo.Id, cts.Token);

                OpenChatForm(roomInfo.Id, CoreRegistryConstants.LocalConnectionId, isHost: true, new NamedPipeEndpoint()
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
                participantConnectionRegistry.RegisterLocalParticipant(roomInfo.Id, localParticipant);
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
                await CleanUpAsync(roomId, connectionId, isHost);
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
                participantConnectionRegistry.Unregister(roomId, connectionId);
            }

            participantStore.ClearParticipants(roomId);
            messageStore.ClearMessages(roomId);
            roomStore.Remove(roomId);
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

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
