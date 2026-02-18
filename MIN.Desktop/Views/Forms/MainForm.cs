using MIN.Desktop.Components;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Services;

namespace MIN.Desktop
{
    public partial class MainForm : StyledForm
    {
        private readonly IChatRoomService chatRoomService;
        private ILocalNetworkComputerProvider networkComputerProvider;
        private readonly SynchronizationContext uiContext;
        private readonly CancellationTokenSource cancellationTokenSource = new();

        public MainForm(IChatRoomService chatRoomService)
        {
            InitializeComponent();

            this.chatRoomService = chatRoomService;

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
                if (!await OnRoomConnection(createdRoom))
                {
                    await chatRoomService.DisconnectAsync();
                }
            }
        }

        private async Task PerfromSearch()
        {
            uiContext.Post(_ => findRooms.Enabled = false, null);

            try
            {
                //var availablePCs = networkComputerProvider.GetLocalNetworkComputerNames(classNumber.Value.ToString());
                //var availablePCs = new List<string>() { AppUserProvider.Instance.CurrentUser.PCName };
                var availablePCs = new List<string>() { "C31203", "C31204" };
                var discoveredRooms = await chatRoomService.DiscoverAvailableRoomsAsync(availablePCs, cancellationToken: cancellationTokenSource.Token);

                totalRoomsCount.Text = $"Всего нашлось комнат: {discoveredRooms.Count()}";

                // Обновляем UI списком найденных комнат
                uiContext.Post(_ => UpdateDiscoveredRoomsList(discoveredRooms), null);
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

        private void UpdateDiscoveredRoomsList(IEnumerable<Room> rooms)
        {
            flowLayoutPanel.Controls.Clear();

            foreach (var room in rooms)
            {
                var card = new RoomCard(chatRoomService, room);
                card.Parent = flowLayoutPanel;
                card.Clicked += () => OnRoomConnection(room);
                card.Disposed += (s, e) => card.UnsubscribeFromChatEvents();
            }
        }

        private async void findRooms_Click(object sender, EventArgs e)
        {
            await PerfromSearch();
        }

        private async Task<bool> OnRoomConnection(Room room)
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

            if (room.CurrentParticipants.Any(p => p.PCName == AppUserProvider.Instance.CurrentUser.PCName))
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
                   await chatRoomService.JoinRoomAsync(room, AppUserProvider.Instance.CurrentUser, cancellationTokenSource.Token);
                } 
                catch(Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }

                var chatForm = new ChatForm(chatRoomService, room);
                chatForm.Show();

                return true;
            }

            return false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            AppUserProvider.Instance.InitializeUser(new Participant()
            {
                Name = "Хост",
                PCName = Environment.MachineName,
            });
        }
    }
}
