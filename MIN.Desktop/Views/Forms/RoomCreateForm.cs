using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;
using MIN.Services.Contracts.Models;

namespace MIN.Desktop
{
    public partial class RoomCreateForm : StyledForm
    {
        /// <summary>
        /// Редактируемая комната
        /// </summary>
        public Room Room { get; set; }

        public RoomCreateForm(Room? room = null)
        {
            Room = room ?? new Room();

            InitializeComponent();
        }

        protected override void ApplyStylings()
        {
            splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
            splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        }

        private bool IsRoomValid()
        {
            return !string.IsNullOrEmpty(Room.Name);
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            Room.Name = roomName.Text;
            Room.MaximumParticipants = Convert.ToInt32(roomMaximumCount.Value);
            Room.HostParticipant = AppUserProvider.Instance.CurrentUser;

            if (!IsRoomValid())
            {
                MessageBox.Show(
                    "Имя комнаты не может быть пустым", 
                    "Ошибка валидации", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation
                );

                return;
            }

            DialogResult = DialogResult.OK;

        }
    }
}
