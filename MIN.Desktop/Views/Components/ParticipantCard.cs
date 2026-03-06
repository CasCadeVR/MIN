using MIN.Desktop.Contracts;
using MIN.Services.Contracts.Models;

namespace MIN.Desktop.Components
{
    /// <summary>
    /// Кнопка меню
    /// </summary>
    public partial class ParticipantCard : UserControl
    {
        private readonly Participant participant;
        private readonly Room room;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public ParticipantCard(Participant participant, Room room)
        {
            InitializeComponent();
            ApplyStylings();
            this.participant = participant;
            this.room = room;
            FillLabels();
        }

        private void ApplyStylings()
        {
            participantName.Font = FontScheme.Monospace;
            lastOnline.Font = FontScheme.Caption;
            participantRole.Font = FontScheme.Monospace;

            tableLayoutPanelLabels.BackColor = ColorScheme.IncomingMessageBackground;
            Dock = DockStyle.Top;
        }

        private void FillLabels()
        {
            participantName.Text = participant.Name;
            if (participant.PCName == room.HostParticipant.PCName)
            {
                participantRole.Text = "Хост";
            }
            else
            {
                participantRole.Text = "";
            }

            var lastMessage = room.ChatHistory
                .Where(x => x.SenderPCName == participant.PCName)
                .OrderByDescending(x => x.Time)
                .LastOrDefault();

            lastOnline.Text = lastMessage == null ? "" : $"Последний раз в сети: {lastMessage.Time.ToShortTimeString()}";
        }
    }
}
