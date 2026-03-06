using MIN.Desktop.Components;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Services;

namespace MIN.Desktop
{
    public partial class MainForm : StyledForm
    {
        private readonly IChatRoomService chatRoomService;
        private readonly ISettingsProvider settingsProvider;
        private readonly INotificationService notificationService;
        private readonly ILoggerProvider loggerProvider;
        private readonly ILocalNetworkComputerProvider networkComputerProvider;
        private readonly SynchronizationContext uiContext;
        private readonly CancellationTokenSource cancellationTokenSource = new();

        private Settings Settings => settingsProvider.GetSettings();

        public MainForm(IChatRoomService chatRoomService, ISettingsProvider settingsProvider, INotificationService notificationService, ILoggerProvider loggerProvider)
        {
            InitializeComponent();

            this.chatRoomService = chatRoomService;
            this.settingsProvider = settingsProvider;
            this.loggerProvider = loggerProvider;
            this.notificationService = notificationService;

            uiContext = SynchronizationContext.Current
                ?? throw new InvalidOperationException("Must be created on UI thread");
            networkComputerProvider = new CollegeNetworkComputerProvider();
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

                var createdRoom = await chatRoomService.CreateRoomAsync(room.Name, room.MaximumParticipants, AppUserProvider.Instance.CurrentUser, cancellationTokenSource.Token);

                //await PerfromSearch(CancellationToken.None);
                if (!await OnRoomConnection(createdRoom, createdRoom.CurrentParticipants.Count))
                {
                    await chatRoomService.DisconnectAsync();
                }
            }
        }

        private async Task PerformSearch()
        {
            uiContext.Post(_ => findRooms.Enabled = false, null);

            try
            {
                var availablePCs = Settings.SearchMethod == SearchMethod.ClassRoom
                    ? networkComputerProvider.GetLocalNetworkComputerNames(classNumber.Value.ToString())
                    : Settings.PreferredPCNames;

                flowLayoutPanel.Controls.Clear();
                var roomsCount = 0;
                await foreach (var room in chatRoomService.DiscoverAvailableRoomsAsync(availablePCs, Settings.DiscoveryTimeout, cancellationToken: cancellationTokenSource.Token))
                {
                    roomsCount += 1;
                    AddDiscoveredRoom(room);
                    totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
                }
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

        private void AddDiscoveredRoom(DiscoveredRoom room)
        {
            var parsed = new Room(room.RoomName, room.MaximumParticipants)
            {
                Id = room.RoomId,
                HostParticipant = new Participant()
                {
                    Id = room.HostId,
                    Name = room.HostName,
                    PCName = room.HostPCName,
                }
            };

            var card = new RoomCard(chatRoomService, room)
            {
                Parent = flowLayoutPanel
            };
            card.Clicked += () => OnRoomConnection(parsed, room.CurrentParticipants);
            card.Disposed += (s, e) =>
            {
                card.UnsubscribeFromChatEvents();
                totalRoomsCount.Text = $"Всего нашлось комнат: {flowLayoutPanel.Controls.Count}";
            };
        }

        private async void findRooms_Click(object sender, EventArgs e)
        {
            await PerformSearch();
        }

        private async Task<bool> OnRoomConnection(Room room, int currentParticipantCount)
        {
            if (room == null)
            {
                MessageBox.Show("Невозможно подключиться: комнаты больше нет.", "Ошибка",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (room.IsFull)
            {
                MessageBox.Show("Невозможно подключиться: комната заполнена.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (room.HostParticipant.PCName == AppUserProvider.Instance.CurrentUser.PCName
                && currentParticipantCount != 0)
            {
                MessageBox.Show("Вы уже подключены к этой комнате.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            var participantCreateForm = new ParticipantCreateForm();
            if (participantCreateForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var chatForm = new ChatForm(chatRoomService, notificationService);
                    await chatRoomService.JoinRoomAsync(room, AppUserProvider.Instance.CurrentUser, Settings.DiscoveryTimeout, cancellationTokenSource.Token);
                    chatForm.FormClosing += (sender, e) =>
                    {
                        chatRoomService.DisconnectAsync(cancellationTokenSource.Token);
                    };
                    chatForm.Show();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }

                return true;
            }

            return false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var selfParticipant = new Participant()
            {
                Name = Environment.MachineName,
                PCName = Environment.MachineName,
            };
            AppUserProvider.Instance.InitializeUser(selfParticipant);

            if (CollegePCNameParser.TryParseComputerName(selfParticipant.PCName, out var roomNumber, out var _))
            {
                classNumber.Value = roomNumber;
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(settingsProvider.GetSettings(), loggerProvider);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                settingsProvider.SaveSettings(settingsForm.Settings);
            }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            await chatRoomService.DisconnectAsync(CancellationToken.None);
        }
    }
}
