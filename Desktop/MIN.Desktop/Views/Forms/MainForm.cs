using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Discovery.Events;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models;
using MIN.Helpers.Services;

namespace MIN.Desktop
{
    public partial class MainForm : StyledForm
    {
        private readonly IRoomConnector roomConnector;
        private readonly IRoomHoster roomHoster;
        private readonly IRoomRegistry roomRegistry;
        private readonly IChatService chatService;
        private readonly IDiscoveryService discoveryService;
        private readonly IEventBus eventBus;
        private readonly IMessageSender messageSender;
        private readonly IMessageEncryptor encryptor;
        private readonly ISettingsProvider settingsProvider;
        private readonly INotificationService notificationService;
        private readonly ILoggerProvider logger;
        private readonly ILocalNetworkComputerProvider networkComputerProvider;
        private readonly IIdentityService identityService;

        private readonly SynchronizationContext uiContext;
        private readonly CancellationTokenSource cts;

        private Settings Settings => settingsProvider.GetSettings();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MainForm"/>
        /// </summary>
        public MainForm(
            IRoomConnector roomConnector,
            IRoomHoster roomHoster,
            IRoomRegistry roomRegistry,
            IChatService chatService,
            IDiscoveryService discoveryService,
            IEventBus eventBus,
            IMessageSender messageSender,
            IMessageEncryptor encryptor,
            ISettingsProvider settingsProvider,
            INotificationService notificationService,
            IIdentityService identityService,
            ILocalNetworkComputerProvider networkComputerProvider,
            ILoggerProvider logger)
        {
            InitializeComponent();

            this.roomConnector = roomConnector;
            this.roomHoster = roomHoster;
            this.roomRegistry = roomRegistry;
            this.chatService = chatService;
            this.discoveryService = discoveryService;
            this.eventBus = eventBus;
            this.messageSender = messageSender;
            this.encryptor = encryptor;
            this.settingsProvider = settingsProvider;
            this.notificationService = notificationService;
            this.identityService = identityService;
            this.networkComputerProvider = networkComputerProvider;
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
                roomCreateForm.Room.HostParticipant = new ParticipantInfo(identityService.SelfPartcipant);
                var room = roomCreateForm.Room;

                roomRegistry.RegisterRoom(Guid.NewGuid(), room);

                var endpoint = new NamedPipeEndpoint(Environment.MachineName, PipeNameProvider.GetRoomPipeName(room.Id));
                await roomHoster.StartHostingAsync(room.Id, endpoint, cts.Token);
                await discoveryService.StartDiscoveryAsync(new RoomInfo(room), cts.Token);
            }
        }

        private async Task PerformSearch()
        {
            uiContext.Post(_ => findRooms.Enabled = false, null);

            try
            {
                flowLayoutPanel.Controls.Clear();
                totalRoomsCount.Text = "Поиск комнат...";

                await discoveryService.DiscoverRoomsAsync(classNumber.Value.ToString(), TimeSpan.FromMilliseconds(Settings.DiscoveryTimeout), cts.Token);
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
                        roomRegistry,
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
            var selfName = Environment.MachineName;

            identityService.SetParticipant(new ParticipantInfo(selfName));

            if (CollegePCNameParser.TryParseComputerName(selfName, out var roomNumber, out var _))
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
