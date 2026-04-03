using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Interfaces.Stores;
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
    public partial class MainForm : StyledForm
    {
        private readonly IRoomConnector roomConnector;
        private readonly IRoomHoster roomHoster;
        private readonly IRoomStore roomStore;
        private readonly IMessageStore messageStore;
        private readonly IParticipantStore participantStore;
        private readonly IRoomConnectionRegistry roomConnectionRegistry;
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
        private ParticipantInfo localParticipant;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MainForm"/>
        /// </summary>
        public MainForm(
            IRoomConnector roomConnector,
            IRoomHoster roomHoster,
            IRoomStore roomStore,
            IMessageStore messageStore,
            IParticipantStore participantStore,
            IRoomConnectionRegistry roomConnectionRegistry,
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
            this.messageStore = messageStore;
            this.participantStore = participantStore;
            this.roomConnectionRegistry = roomConnectionRegistry;
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
            eventBus.Subscribe<ConnectionStatusChangedEvent>(OnConnectionStatusChanged);
        }

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
            if (roomCreateForm.ShowDialog() == DialogResult.OK)
            {
                var room = roomCreateForm.Room;
                localParticipant.Endpoint = new NamedPipeEndpoint(Environment.MachineName, PipeNameProvider.GetRoomPipeName(room.Id));
                room.HostParticipant = localParticipant;

                try
                {
                    roomStore.Add(room);
                    messageStore.AddMessage(room.Id, new SystemTextMessage()
                    {
                        Content = $"Комната {room.Name} была создана в {DateTime.Now.ToShortTimeString()}"
                    });
                    roomConnectionRegistry.Associate(Guid.NewGuid(), room.Id);
                    await roomHoster.StartHostingAsync(room.Id, room.HostParticipant.Endpoint, cts.Token);

                    var roomInfo = new RoomInfo(room);

                    await discoveryService.StartDiscoveryAsync(roomInfo.Id, cts.Token);
                    await OnRoomJoin(roomInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Создание комнаты прошло не успешно: {ex.Message}", "Error");
                }
            }
        }

        private async Task PerformSearch()
        {
            uiContext.Post(_ => findRooms.Enabled = false, null);

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
                uiContext.Post(_ => findRooms.Enabled = true, null);
            }
        }

        private Task OnRoomDiscovered(RoomDiscoveredEvent e, CancellationToken ct)
        {
            uiContext.Post(_ =>
            {
                var card = new RoomCard(e.Room)
                {
                    Parent = flowLayoutPanel
                };

                card.Clicked += () => OnRoomJoin(e.Room);
                card.Disposed += (s, _) =>
                {
                    totalRoomsCount.Text = $"Всего нашлось комнат: {flowLayoutPanel.Controls.Count}";
                };

                var roomsCount = flowLayoutPanel.Controls.Count;
                totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
            }, null);

            return Task.CompletedTask;
        }

        private async Task OnRoomJoin(RoomInfo roomInfo)
        {
            if (roomInfo == null)
            {
                MessageBox.Show("Невозможно подключиться: комната не найдена.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (roomInfo.ParticipantCount >= roomInfo.MaximumParticipants)
            {
                MessageBox.Show("Невозможно подключиться: комната заполнена.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var participantCreateForm = new ParticipantCreateForm(identityService);
            if (participantCreateForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var connectionId = await roomConnector.ConnectAsync(roomInfo.Id, roomInfo.HostParticipant.Endpoint!, Settings.DiscoveryTimeout, cts.Token);

                    var chatForm = new ChatForm(
                       chatService,
                       messageStore,
                       participantStore,
                       roomStore,
                       eventBus,
                       notificationService,
                       logger,
                       identityService,
                       roomInfo.Id,
                       connectionId);

                    chatForm.FormClosing += (_, _) =>
                    {
                        roomConnector.DisconnectAsync(roomInfo.Id, connectionId);
                    };
                    chatForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
            }
        }

        private async void findRooms_Click(object sender, EventArgs e)
        {
            await PerformSearch();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var machineName = Environment.MachineName;

            localParticipant = new ParticipantInfo(machineName);

            identityService.SetParticipant(localParticipant);

            if (CollegePCNameParser.TryParseComputerName(machineName, out var roomNumber, out var _))
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
            await discoveryService.StopDiscoveryAsync();
            await roomHoster.StopHostingAsync(Guid.Empty);
            await roomConnector.DisconnectAsync(Guid.Empty, Guid.Empty);
            cts.Cancel();
            cts.Dispose();
        }

        private Task OnConnectionStatusChanged(ConnectionStatusChangedEvent eventMessage, CancellationToken ct)
        {
            uiContext.Post(_ =>
            {
                if (!eventMessage.IsConnected)
                {
                    MessageBox.Show($"Соединение с комнатой {eventMessage.RoomId} было потеряно",
                        "Соединение потеряно",
                        MessageBoxButtons.OK,
                        icon: MessageBoxIcon.Error
                    );
                }
            }, null);
            return Task.CompletedTask;
        }
    }
}
