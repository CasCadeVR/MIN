using MIN.Desktop.Contracts;
using MIN.Services.Contracts.Models;

namespace MIN.Desktop.Components
{
    /// <summary>
    /// Кнопка меню
    /// </summary>
    public partial class RoomCard : UserControl
    {
        /// <summary>
        /// Событие по нажатию
        /// </summary>
        public event Func<Room, bool> Clicked;

        private readonly Room room;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public RoomCard(Room room)
        {
            InitializeComponent();
            ApplyStylings();
            this.room = room;
            FillLabels();
        }

        private void ApplyStylings()
        {
            splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
            splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
            tableLayoutPanelLabels.BackColor = ColorScheme.MainPanelBackground;
        }

        private void FillLabels()
        {
            Title.Text = $"Комната {this.room.Name}";
            participantsInfo.Text = $"{this.room.CurrentParticipants.Count}/{this.room.MaximumParticipants}";
            hostName.Text = this.room.HostParticipant.Name;

            // TODO: исправить на № компа и кабинет
            computer.Text = this.room.HostParticipant.PCName;
            classroom.Text = this.room.HostParticipant.PCName;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            Clicked?.Invoke(room);
        }
    }
}