using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Constants;
using MIN.Services.Connection.Pipes;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Services;

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

        private readonly IChatRoomService chatRoomService;
        private readonly SynchronizationContext uiContext;
        private Room room;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public RoomCard(IChatRoomService chatRoomService, Room room)
        {
            InitializeComponent();
            this.chatRoomService = chatRoomService;
            this.room = room;
            ApplyStylings();

            SubscribeToChatEvents();

            UpdateStats();
        }

        private void SubscribeToChatEvents()
        {
            chatRoomService.ParticipantJoined += (s, e) =>
                uiContext.Post(_ => OnParticipantJoined(e.Participant), null);

            chatRoomService.ParticipantLeft += (s, e) =>
                uiContext.Post(_ => OnParticipantLeft(e.Participant), null);

            chatRoomService.RoomStateChanged += (s, e) =>
                uiContext.Post(_ => OnRoomInfoChanged(e.Room!, e.State), null);
        }


        private void UpdateStatsAndInvoke<Entity>(Action<Entity> action, Entity entity)
        {
            if (InvokeRequired)
            {
                Invoke(action, entity);
                return;
            }
            UpdateStats();
        }

        private void OnParticipantJoined(Participant participant)
        {
            UpdateStatsAndInvoke(OnParticipantJoined, participant);
        }

        private void OnParticipantLeft(Participant participant)
        {
            UpdateStatsAndInvoke(OnParticipantJoined, participant);
        }

        private void OnRoomInfoChanged(Room room, RoomState roomState)
        {
            this.room = room;
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
            Title.Text = $"Комната {this.room.Name}";
            participantsInfo.Text = $"{this.room.CurrentParticipants.Count}/{this.room.MaximumParticipants}";
            hostName.Text = this.room.HostParticipant.Name;

            if (CollegePCNameParser.TryParseComputerName(this.room.HostParticipant.PCName, out int roomNumber, out int computerNumber))
            {
                computer.Text = roomNumber.ToString();
                classroom.Text = computerNumber.ToString();
            } 
            else
            {
                computer.Text = DesktopConstants.UndefinedPCName;
                classroom.Text = DesktopConstants.UndefinedPCName;
            }
           
            setConnectButtonAccordingToRoomCount();
        }

        private void setConnectButtonAccordingToRoomCount()
        {
            connectButton.Enabled = !room.IsFull;
            if (room.IsFull)
            {
                connectButton.Text = "Заполнено";
                connectButton.BackColor = ColorScheme.RoomFilled;
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            Clicked?.Invoke(room);
        }
    }
}