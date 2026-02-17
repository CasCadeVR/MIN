using MIN.Desktop.Components;
using MIN.Desktop.Components.Labels;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Components;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Events;
using MIN.Services.Services;

namespace MIN.Desktop
{
    /// <summary>
    /// Форма чата
    /// </summary>
    public partial class ChatForm : StyledForm
    {
        private readonly int startMessageBoxHeight;
        private readonly IChatRoomService chatRoomService;
        private readonly CancellationTokenSource formCancellationTokenSource = new();
        private readonly SynchronizationContext uiContext;

        /// <summary>
        /// Текущая комната
        /// </summary>
        public Room Room { get; set; }

        public ChatForm(IChatRoomService chatRoomService, Room? room = null)
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current
                ?? throw new InvalidOperationException("Must be created on UI thread");

            startMessageBoxHeight = tableLayoutPanelButtons.Height;
            this.chatRoomService = chatRoomService;
            Room = room ?? new Room();

            SubscribeToChatEvents();
            UpdateStats();
            UpdateChatFlow();
        }

        private void SubscribeToChatEvents()
        {
            chatRoomService.MessageReceived += OnMessageRecievedEvent;
            chatRoomService.ParticipantJoined += OnParticipantJoinedEvent;
            chatRoomService.ParticipantLeft += OnParticipantLeftEvent;
            chatRoomService.RoomStateChanged += OnRoomInfoChangedEvent;
            chatRoomService.ConnectionLost += ConnectionLostEvent;
        }

        private void UnsubscribeFromChatEvents()
        {
            chatRoomService.MessageReceived -= OnMessageRecievedEvent;
            chatRoomService.ParticipantJoined -= OnParticipantJoinedEvent;
            chatRoomService.ParticipantLeft -= OnParticipantLeftEvent;
            chatRoomService.RoomStateChanged -= OnRoomInfoChangedEvent;
            chatRoomService.ConnectionLost -= ConnectionLostEvent;
        }

        private void OnMessageRecievedEvent(object? sender, MessageReceivedEventArgs e)
        {
            uiContext.Post(_ => OnMessageReceived(e.Message), null);
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

        private void ConnectionLostEvent(object? sender, ConnectionLostEventArgs e)
        {
            uiContext.Post(_ => OnConnectionLost(e.Reason), null);
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

        private void OnRoomInfoChanged(Room room, RoomState roomState)
        {
            if (roomState == RoomState.Disconnected)
            {
                Close();
                return;
            }

            Room = room;
            UpdateStats();
        }

        private void OnParticipantJoined(Participant participant)
        {
            SendParticipantJoinedMessage(participant);
            UpdateStatsAndInvoke(OnParticipantJoined, participant);
        }

        private void OnParticipantLeft(Participant participant)
        {
            SendParticipantLeftMessage(participant);
            UpdateStatsAndInvoke(OnParticipantJoined, participant);
        }
        
        private void OnConnectionLost(string reason)
        {
            MessageBox.Show(reason, "Подключение разорвано",
                MessageBoxButtons.OK,
                icon: MessageBoxIcon.Error);
            Close();
        }

        private void OnMessageReceived(ChatMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(OnMessageReceived, message);
                return;
            }

            //Room.AddMessage(message);
            UpdateChatFlow();
        }

        private void SendParticipantJoinedMessage(Participant participant)
        {
            var roomMessage = new ChatMessage()
            {
                AsRoomMessage = true,
                Content = $"Участник {participant.Name} зашёл в комнату",
                MessageType = MessageType.Text,
            };

            Room.AddMessage(roomMessage);
        }

        private void SendParticipantLeftMessage(Participant participant)
        {
            var roomMessage = new ChatMessage()
            {
                AsRoomMessage = true,
                Content = $"Участник {participant.Name} покинул комнату",
                MessageType = MessageType.Text,
            };

            Room.AddMessage(roomMessage);
        }

        private void UpdateStats()
        {
            Title.Text = $"Комната {Room.Name}";
            participantsInfo.Text = $"{Room.CurrentParticipants.Count}/{Room.MaximumParticipants}";
            hostName.Text = Room.HostParticipant.Name;

            if (CollegePCNameParser.TryParseComputerName(Room.HostParticipant.PCName, out int roomNumber, out int computerNumber))
            {
                computer.Text = computerNumber.ToString();
                classroom.Text = roomNumber.ToString();
            }
            else
            {
                computer.Text = DesktopConstants.UndefinedPCName;
                classroom.Text = DesktopConstants.UndefinedPCName;
            }

            UpdateParticipantFlow();
        }

        private void UpdateParticipantFlow()
        {
            participantsFlow.Controls.Clear();

            foreach (var participant in Room.CurrentParticipants)
            {
                var card = new ParticipantCard(participant, Room);
                card.MinimumSize = new Size(Width - splitContainerSideBar.SplitterDistance - (card.Margin.Left * 6), card.Height);
                card.Size = card.MinimumSize;
                participantsFlow.Controls.Add(card);
            }
        }

        private void UpdateChatFlow()
        {
            chatFlow.Controls.Clear();

            foreach (var message in Room.ChatHistory)
            {
                var row = new ChatMessageRow();
                Control rowControl;

                if (message.MessageType == MessageType.System
                    || message.MessageType == MessageType.Command)
                {
                    rowControl = new Heading3Label()
                    {
                        Text = message.Content,
                        Height = row.Height,
                        Anchor = AnchorStyles.None,
                    };
                }
                else
                {
                    rowControl = new ChatMessageCard(message)
                    {
                        Anchor = message.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName
                            ? AnchorStyles.Right
                            : AnchorStyles.Left,
                    };
                }

                row.Size = new Size(chatFlow.Width - (row.Margin.Left * 2) - chatFlow.Margin.Left, rowControl.Height);
                row.container.Controls.Add(rowControl);
                chatFlow.Controls.Add(row);
            }
        }

        protected override void ApplyStylings()
        {
            splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
            splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
            splitContainerSideBar.Panel2.BackColor = ColorScheme.PrimaryAccent;
            tableLayoutPanelStats.BackColor = ColorScheme.PrimaryAccent;

            participantsInfo.ForeColor = ColorScheme.TextOnAccent;
            hostName.ForeColor = ColorScheme.TextOnAccent;
            computer.ForeColor = ColorScheme.TextOnAccent;
            classroom.ForeColor = ColorScheme.TextOnAccent;

            heading3Label1.ForeColor = ColorScheme.TextOnAccent;
            heading3Label2.ForeColor = ColorScheme.TextOnAccent;
            heading3Label3.ForeColor = ColorScheme.TextOnAccent;
            heading3Label4.ForeColor = ColorScheme.TextOnAccent;
            heading3Label5.ForeColor = ColorScheme.TextOnAccent;

            participantsFlow.BackColor = ColorScheme.DividerColor;
            chatFlow.BackColor = ColorScheme.ChatAreaBackground;

            editButton.Visible = AppUserProvider.Instance.CurrentUser.PCName == Room.HostParticipant.PCName;
            tableLayoutPanelButtons.RowStyles[0] = new RowStyle(SizeType.AutoSize);
            tableLayoutPanel2.RowStyles[1] = new RowStyle(SizeType.AutoSize);
            changeMessageBoxSize();
        }

        private bool IsMessageValid()
        {
            return !string.IsNullOrWhiteSpace(messageTextBox.Text);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void sendMessage()
        {
            if (!IsMessageValid())
            {
                return;
            }

            chatRoomService.SendMessageAsync(messageTextBox.Text, MessageType.Text);
            messageTextBox.Text = string.Empty;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chatFlow_Resize(object sender, EventArgs e)
        {
            foreach (ChatMessageRow control in chatFlow.Controls)
            {
                control.Width = chatFlow.Width - (control.Margin.Left * 2) - chatFlow.Margin.Left;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            splitContainerSideBar.Panel2Collapsed = true;
            chatFlow_Resize(sender, e);
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            splitContainerSideBar.Panel2Collapsed = !splitContainerSideBar.Panel2Collapsed;
            chatFlow_Resize(sender, e);
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            UnsubscribeFromChatEvents();
            formCancellationTokenSource.Dispose();
            await chatRoomService.DisconnectAsync();
            base.OnFormClosing(e);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var editForm = new RoomCreateForm(Room);
            var result = editForm.ShowDialog();
            if (result == DialogResult.Abort)
            {
                Close();
            }
            else if (result == DialogResult.OK)
            {
                Room.UpdateInfo(Room);
            }
        }

        private void messageTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') // Enter
            {
                if ((ModifierKeys & Keys.Shift) == 0)
                {
                    sendMessage();
                    tableLayoutPanelButtons.Height = startMessageBoxHeight;
                    changeMessageBoxSize();
                    e.Handled = true;
                }
            }
        }

        private void changeMessageBoxSize()
        {
            tableLayoutPanelButtons.Height = messageTextBox.Height + tableLayoutPanelButtons.Margin.Vertical;

        }

        private void messageTextBox_TextChanged(object sender, EventArgs e)
        {
            changeMessageBoxSize();
        }
    }
}
