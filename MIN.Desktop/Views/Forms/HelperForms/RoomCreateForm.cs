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

        private bool isNew = false;

        public RoomCreateForm(Room? room = null)
        {
            isNew = room == null;
            Room = room ?? new Room();

            InitializeComponent();
        }

        protected override void ApplyStylings()
        {
            splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
            splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
            Title.ForeColor = ColorScheme.TextOnAccent;

            if (!isNew)
            {
                cancelButton.FlatAppearance.BorderColor = ColorScheme.ErrorColor;
                cancelButton.ForeColor = ColorScheme.ErrorColor;
                cancelButton.Text = "Удалить комнату";

                createButton.Text = "Сохранить";
                roomName.Text = Room.Name;
                roomMaximumCount.Value = Room.MaximumParticipants;
            }
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

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (isNew)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите удалить эту комнату?", "Удаление комнаты", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DialogResult = DialogResult.Abort;
                }
            }
        }

        private void RoomCreateForm_Load(object sender, EventArgs e)
        {
            roomName.Focus();
        }
    }
}
