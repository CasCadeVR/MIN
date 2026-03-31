using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components
{
    /// <summary>
    /// Кнопка меню
    /// </summary>
    public partial class ParticipantCard : UserControl
    {
        private readonly ParticipantInfo participant;
        private readonly Room room;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public ParticipantCard(ParticipantInfo participant, Room room)
        {
            InitializeComponent();
            ApplyStylings();
            this.participant = participant;
            this.room = room;
            FillLabels();
        }

        private void ApplyStylings()
        {
            participantName.Font = FontScheme.Caption;
            lastOnline.Font = FontScheme.MicroCaption;
            participantRole.Font = FontScheme.Caption;

            tableLayoutPanelLabels.BackColor = ColorScheme.IncomingMessageBackground;
        }

        private void FillLabels()
        {
            participantName.Text = participant.Name;
            if (participant.Id == room.HostParticipant.Id)
            {
                participantRole.Text = "Хост";
            }
            else
            {
                participantRole.Text = "";
                tableLayoutPanelLabels.ColumnStyles[1].Width = 0;
            }

            //var lastMessage = room.ChatHistory
            //    .Where(x => x. == participant.Id)
            //    .OrderByDescending(x => x.Time)
            //    .LastOrDefault();

            //lastOnline.Text = lastMessage == null ? "" : $"Последний раз в сети: {lastMessage.Time.ToShortTimeString()}";
        }
    }
}
