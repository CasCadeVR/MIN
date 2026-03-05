using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Constants;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Events;
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
        public event Func<Task> Clicked;

        private readonly IChatRoomService chatRoomService;
        private readonly SynchronizationContext uiContext;
        private DiscoveredRoom room;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public RoomCard(IChatRoomService chatRoomService, DiscoveredRoom room)
        {
            InitializeComponent();
            this.chatRoomService = chatRoomService;
            this.room = room;
            uiContext = SynchronizationContext.Current
                ?? throw new InvalidOperationException("Must be created on UI thread");

            ApplyStylings();
            SubscribeToChatEvents();
            UpdateStats();
        }

        private void SubscribeToChatEvents()
        {
            chatRoomService.ParticipantJoined += OnParticipantJoinedEvent;
            chatRoomService.ParticipantLeft += OnParticipantLeftEvent;
            chatRoomService.RoomStateChanged += OnRoomInfoChangedEvent;
        }

        /// <summary>
        /// Отключить события
        /// </summary>
        public void UnsubscribeFromChatEvents()
        {
            chatRoomService.ParticipantJoined -= OnParticipantJoinedEvent;
            chatRoomService.ParticipantLeft -= OnParticipantLeftEvent;
            chatRoomService.RoomStateChanged -= OnRoomInfoChangedEvent;
        }

        private void OnParticipantJoinedEvent(object? sender, ParticipantJoinedEventArgs e)
        {
            uiContext.Post(_ => OnParticipantJoined(e.Participant), null);
        }

        private void OnParticipantLeftEvent(object? sender, ParticipantLeftEventArgs e)
        {
            uiContext.Post(_ => OnParticipantLeft(e.Participant), null);
        }

        private void OnRoomInfoChangedEvent(object? sender, RoomStateChangedEventArgs e)
        {
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
            room.CurrentParticipants++;
            UpdateStatsAndInvoke(OnParticipantJoined, participant);
        }

        private void OnParticipantLeft(Participant participant)
        {
            room.CurrentParticipants--;
            UpdateStatsAndInvoke(OnParticipantJoined, participant);
        }

        private void OnRoomInfoChanged(Room room, RoomState roomState)
        {
            if (roomState == RoomState.Disconnected)
            {
                Dispose();
                return;
            }

            this.room.RoomName = room.Name;
            this.room.HostName = room.HostParticipant.Name;
            this.room.HostPCName = room.HostParticipant.PCName;
            this.room.CurrentParticipants = room.CurrentParticipants.Count;
            room.MaximumParticipants = room.MaximumParticipants;
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
            Title.Text = $"Комната {room.RoomName}";
            participantsInfo.Text = $"{room.CurrentParticipants}/{room.MaximumParticipants}";
            hostName.Text = room.HostName;

            if (CollegePCNameParser.TryParseComputerName(room.HostPCName, out int roomNumber, out int computerNumber))
            {
                computer.Text = computerNumber.ToString();
                classroom.Text = roomNumber.ToString();
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
            var isFull = room.CurrentParticipants >= room.MaximumParticipants;

            connectButton.Enabled = !isFull;
            if (isFull)
            {
                connectButton.Text = "Заполнено";
                connectButton.BackColor = ColorScheme.RoomFilled;
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
