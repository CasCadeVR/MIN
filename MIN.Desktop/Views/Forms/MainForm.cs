using MIN.Desktop.Components;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;

namespace MIN.Desktop
{
    public partial class MainForm : StyledForm
    {
        private readonly IRoomService roomService;

        public MainForm(IRoomService roomService)
        {
            InitializeComponent();
            this.roomService = roomService;
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
                var room = await roomService.Create(roomCreateForm.Room, CancellationToken.None);
                // TODO: server creation here

                await PerfromSearch(CancellationToken.None);
                if (!OnRoomConnection(room))
                {
                    await roomService.Delete(room, CancellationToken.None);
                }
            }
        }

        private async Task PerfromSearch(CancellationToken cancellationToken)
        {
            var foundRooms = await roomService.GetAll(CancellationToken.None);
            totalRoomsCount.Text = $"Всего нашлось комнат: {foundRooms.Count()}";

            foreach (var room in foundRooms)
            {
                var card = new RoomCard(room);
                card.Parent = flowLayoutPanel;
                card.Clicked += (room) => OnRoomConnection(room);
            }
        }

        private async void findRooms_Click(object sender, EventArgs e)
        {
            await PerfromSearch(CancellationToken.None);
        }

        private bool OnRoomConnection(Room room)
        {
            var participantCreateForm = new ParticipantCreateForm();
            if (participantCreateForm.ShowDialog() == DialogResult.OK)
            {
                var chatForm = new ChatForm(room);
                chatForm.Show();

                // TODO: открыть чат
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
