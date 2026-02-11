using MIN.Desktop.Components;
using MIN.Desktop.Components.Labels;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Components;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;

namespace MIN.Desktop
{
    /// <summary>
    /// Форма чата
    /// </summary>
    public partial class ChatForm : StyledForm
    {
        private readonly IRoomService roomService;

        /// <summary>
        /// Текущая комната
        /// </summary>
        public Room Room { get; set; }

        public ChatForm(IRoomService roomService, Room? room = null)
        {
            InitializeComponent();
            this.roomService = roomService;
            Room = room ?? new Room();

            Room.ParticipantJoined += OnParticipantJoined;
            Room.ParticipantLeft += OnParticipantLeft;
            Room.MessageReceived += OnMessageReceived;
            Room.RoomInfoChanged += OnRoomInfoChanged;

            UpdateStats();
            UpdateChatFlow();
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
            SendParticipantJoinedMessage(participant);
            UpdateStatsAndInvoke(OnParticipantJoined, participant);
        }

        private void OnParticipantLeft(Participant participant)
        {
            SendParticipantLeftMessage(participant);
            UpdateStatsAndInvoke(OnParticipantJoined, participant);
        }

        private void OnRoomInfoChanged(Room room)
        {
            UpdateStatsAndInvoke(OnRoomInfoChanged, room);
        }

        private void OnMessageReceived(ChatMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(OnMessageReceived, message);
                return;
            }
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

            // TODO: исправить на № компа и кабинет
            computer.Text = Room.HostParticipant.PCName;
            classroom.Text = Room.HostParticipant.PCName;

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

                if (message.AsRoomMessage)
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
        }

        private bool IsMessageValid(ChatMessage message)
        {
            return !string.IsNullOrWhiteSpace(message.Content);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void sendMessage()
        {
            var message = new ChatMessage()
            {
                SenderName = AppUserProvider.Instance.CurrentUser.Name,
                SenderPCName = AppUserProvider.Instance.CurrentUser.PCName,
                Content = messageTextBox.Text,
                MessageType = MessageType.Text,
            };

            if (!IsMessageValid(message))
            {
                return;
            }

            Room.AddMessage(message);
            messageTextBox.Text = string.Empty;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            // TODO: disconnect client somehow
            Room.RemoveParticipant(AppUserProvider.Instance.CurrentUser);
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

        private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Room.ParticipantJoined -= OnParticipantJoined;
            Room.ParticipantLeft -= OnParticipantLeft;
            Room.MessageReceived -= OnMessageReceived;
            Room.RoomInfoChanged += OnRoomInfoChanged;
        }

        private async void editButton_Click(object sender, EventArgs e)
        {
            var editForm = new RoomCreateForm(Room);
            var result = editForm.ShowDialog();
            if (result == DialogResult.Abort)
            {
                await roomService.Delete(Room, CancellationToken.None);
                Close();
            }
            else if (result == DialogResult.OK)
            {
                Room = await roomService.Update(Room, CancellationToken.None);
                Room.UpdateStats(Room);
            }
        }

        private void messageTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') // Enter
            {
                if ((ModifierKeys & Keys.Shift) == 0)
                {
                    sendMessage();
                    e.Handled = true;
                }
            }
        }
    }
}
