using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Constants;
using MIN.Helpers.Services;

namespace MIN.Desktop.Components
{
    /// <summary>
    /// Кнопка меню
    /// </summary>
    public partial class RoomCard : UserControl
    {
        private readonly RoomInfo room;
        private readonly ParticipantInfo localParticipant;

        /// <summary>
        /// Событие по нажатию
        /// </summary>
        public event Func<Task> Clicked = null!;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public RoomCard(ParticipantInfo localParticipant, RoomInfo room)
        {
            InitializeComponent();
            this.localParticipant = localParticipant;
            this.room = room;

            ApplyStylings();
            UpdateStats();
        }

        private void ApplyStylings()
        {
            splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
            splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
            tableLayoutPanelLabels.BackColor = ColorScheme.MainPanelBackground;
            Title.ForeColor = ColorScheme.TextOnAccent;
        }

        private void UpdateStats()
        {
            Title.Text = $"Комната {room.Name}";
            participantsInfo.Text = $"{room.ParticipantCount}/{room.MaximumParticipants}";
            hostName.Text = room.HostParticipant.Name;
            createdAt.Text = room.CreatedAt.ToShortTimeString();

            if (CollegePCNameParser.TryParseComputerName(room.HostParticipant.Name, out int roomNumber, out int computerNumber))
            {
                computer.Text = computerNumber.ToString();
                classroom.Text = roomNumber.ToString();
            }
            else
            {
                computer.Text = DesktopConstants.UndefinedPCName;
                classroom.Text = DesktopConstants.UndefinedPCName;
            }

            manageConnectButtonAccessability();
        }

        private void manageConnectButtonAccessability()
        {
            var isFull = room.ParticipantCount >= room.MaximumParticipants;
            var isOwner = room.HostParticipant.Id == localParticipant.Id;
            var isNotAccessible = isFull || isOwner;

            connectButton.Enabled = !isNotAccessible;

            if (isNotAccessible)
            {
                connectButton.Text = isFull ? "Заполнено" : "Твоя комната";
                connectButton.BackColor = ColorScheme.ConnectionDisabled;
            }
            else
            {
                connectButton.Text = "Присоединиться";
                connectButton.BackColor = ColorScheme.SecondaryAccent;
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            Clicked?.Invoke();
        }
    }
}
