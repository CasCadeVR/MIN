using MIN.Desktop.Components;
using MIN.Desktop.Components.Labels;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Components;
using MIN.Services.Contracts.Constants;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Events;
using MIN.Services.Contracts.Models.Messages;
using MIN.Services.Services;

namespace MIN.Desktop
{
    /// <summary>
    /// Форма чата
    /// </summary>
    public partial class ChatForm : StyledForm
    {
        private readonly IChatRoomService chatRoomService;
        private readonly INotificationService notificationService;
        private readonly CancellationTokenSource formCancellationTokenSource = new();
        private readonly SynchronizationContext uiContext;
        private readonly int hideSideBarWidth;
        private readonly System.Windows.Forms.Timer resizeTimer = new() { Interval = 150 };

        private bool isResizing;
        private Room room = null!;
        private ChatMessage lastMessage = null!;

        public ChatForm(IChatRoomService chatRoomService, INotificationService notificationService)
        {
            InitializeComponent();
            SendLoadingMessage();

            uiContext = SynchronizationContext.Current
                ?? throw new InvalidOperationException("Must be created on UI thread");
            resizeTimer.Tick += (s, e) =>
            {
                resizeTimer.Stop();
                isResizing = false;
                PerformResize();
            };

            this.chatRoomService = chatRoomService;
            this.notificationService = notificationService;
            hideSideBarWidth = MinimumSize.Width + splitContainerSideBar.Panel2.Width;

            SubscribeToChatEvents();
        }

        private void SubscribeToChatEvents()
        {
            chatRoomService.MessageReceived += OnMessageRecievedEvent;
            chatRoomService.ParticipantJoined += OnParticipantJoinedEvent;
            chatRoomService.ParticipantLeft += OnParticipantLeftEvent;
            chatRoomService.RoomStateChanged += OnRoomStateChangedEvent;
            chatRoomService.ConnectionLost += ConnectionLostEvent;

            notificationService.OnNotificationClick += () =>
            {
                WindowState = FormWindowState.Normal;
                Focus();
            };
            notificationService.NotificationTurnOffClicked += () => notificationComboBox.Checked = false;
        }

        private void UnsubscribeFromChatEvents()
        {
            chatRoomService.MessageReceived -= OnMessageRecievedEvent;
            chatRoomService.ParticipantJoined -= OnParticipantJoinedEvent;
            chatRoomService.ParticipantLeft -= OnParticipantLeftEvent;
            chatRoomService.RoomStateChanged -= OnRoomStateChangedEvent;
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

        private void OnRoomStateChangedEvent(object? sender, RoomStateChangedEventArgs e)
        {
            uiContext.Post(_ => OnRoomStateChanged(e.Room!, e.State), null);
        }

        private void ConnectionLostEvent(object? sender, ConnectionLostEventArgs e)
        {
            uiContext.Post(_ => OnConnectionLost(e.Reason), null);
        }

        private void UpdateStatsAndInvoke()
        {
            if (InvokeRequired)
            {
                Invoke(UpdateStatsAndInvoke);
                return;
            }

            UpdateStats();
        }

        private void OnRoomStateChanged(Room room, RoomState roomState)
        {
            if (roomState == RoomState.Disconnected)
            {
                Close();
                return;
            }

            this.room = room;
            if (roomState == RoomState.Joined)
            {
                UpdateChatFlow();
            }
            UpdateStats();
        }

        private void OnParticipantJoined(Participant participant)
        {
            SendParticipantJoinedMessage(participant);
            UpdateStatsAndInvoke();
        }

        private void OnParticipantLeft(Participant participant)
        {
            SendParticipantLeftMessage(participant);
            UpdateStatsAndInvoke();
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

            if (notificationComboBox.Checked && (WindowState == FormWindowState.Minimized || !ContainsFocus))
            {
                notificationService.Notify(message, room.Name);
            }
            AddMessageToChatFlow(message);
        }

        private void SendParticipantJoinedMessage(Participant participant)
        {
            var roomMessage = new ChatMessage()
            {
                Content = $"Участник {participant.Name} зашёл в комнату",
                MessageType = MessageType.System,
            };

            if (notificationComboBox.Checked && (WindowState == FormWindowState.Minimized || !ContainsFocus))
            {
                notificationService.Notify(roomMessage, room.Name);
            }
            AddMessageToChatFlow(roomMessage);
        }

        private void SendParticipantLeftMessage(Participant participant)
        {
            var roomMessage = new ChatMessage()
            {
                Content = $"Участник {participant.Name} покинул комнату",
                MessageType = MessageType.System,
            };

            if (notificationComboBox.Checked && (WindowState == FormWindowState.Minimized || !ContainsFocus))
            {
                notificationService.Notify(roomMessage, room.Name);
            }
            AddMessageToChatFlow(roomMessage);
        }

        private void SendLoadingMessage()
        {
            var roomMessage = new ChatMessage()
            {
                Content = "Загрузка...",
                MessageType = MessageType.System,
            };

            AddMessageToChatFlow(roomMessage);
        }

        private void UpdateStats()
        {
            Text = $"MIN - Комната {room.Name}";
            Title.Text = $"Комната {room.Name}";
            participantsInfo.Text = $"{room.CurrentParticipants.Count}/{room.MaximumParticipants}";
            hostName.Text = AppUserProvider.Instance.CurrentUser.PCName == room.HostParticipant.PCName ? "Ты" : room.HostParticipant.Name;
            editButton.Visible = AppUserProvider.Instance.CurrentUser.PCName == room.HostParticipant.PCName;

            if (CollegePCNameParser.TryParseComputerName(room.HostParticipant.PCName, out var roomNumber, out var computerNumber))
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

            foreach (var participant in room.CurrentParticipants)
            {
                var card = new ParticipantCard(participant, room);
                card.Width = participantsFlow.Width - participantsFlow.Margin.Horizontal * 2;
                participantsFlow.Controls.Add(card);
            }
        }

        private void UpdateChatFlow()
        {
            chatFlow.Controls.Clear();

            foreach (var message in room.ChatHistory)
            {
                if (message.MessageType == MessageType.System)
                {
                    if (message.Content.StartsWith("JOIN:"))
                    {
                        var participant = ParseParticipantFromMessage(message);
                        SendParticipantJoinedMessage(participant);
                        continue;
                    }
                    else if (message.Content.StartsWith("LEAVE:"))
                    {
                        var participant = ParseParticipantFromMessage(message);
                        SendParticipantLeftMessage(participant);
                        continue;
                    }
                }

                AddMessageToChatFlow(message);
            }
        }

        private Participant ParseParticipantFromMessage(ChatMessage message)
        {
            var parts = message.Content.Split(':', 3);
            var name = parts.Length > 1 ? parts[1] : "Unknown";
            var id = parts.Length > 2 ? Guid.Parse(parts[2]) : Guid.NewGuid();

            return new Participant
            {
                Id = id,
                Name = name,
                PCName = message.SenderPCName
            };
        }

        private void AddMessageToChatFlow(ChatMessage message)
        {
            chatFlow.SuspendLayout();
            try
            {
                var row = new ChatMessageRow();
                Control rowControl = null!;

                if (message.MessageType == MessageType.System)
                {
                    rowControl = new PrimaryLabel()
                    {
                        Text = message.Content,
                        Anchor = AnchorStyles.None,
                    };

                    row.Height = rowControl.Height;
                }
                else if (message.MessageType == MessageType.Text)
                {
                    var isSelfMessage = message.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName;

                    var minutesPassed = 0;

                    if (lastMessage != null)
                    {
                        isSelfMessage |= lastMessage.SenderPCName == message.SenderPCName;

                        minutesPassed = (message.Time - lastMessage.Time).Minutes;
                        minutesPassed = minutesPassed > 4 ? 8 : minutesPassed + 4;
                    }

                    rowControl = new ChatMessageCard(message, room.HostParticipant.PCName == message.SenderPCName, removeHeaders: isSelfMessage)
                    {
                        Anchor = message.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName
                            ? AnchorStyles.Right
                            : AnchorStyles.Left,
                        Margin = new Padding(20, 0, 20, 0)
                    };

                    row.Margin = new Padding(row.Margin.Left, minutesPassed, row.Margin.Right, row.Margin.Bottom);

                    lastMessage = message;
                }

                row.Width = chatFlow.Width;
                row.container.Controls.Add(rowControl);
                chatFlow.Controls.Add(row);
                chatFlow.Controls.SetChildIndex(chatFlow.Controls[chatFlow.Controls.Count - 1], 0);

                if (rowControl is ChatMessageCard card)
                {
                    row.Height = card.ResizeOutOfPrefferedSize();
                }
            }
            finally
            {
                chatFlow.ResumeLayout(true);
                chatFlow.VerticalScroll.Value = chatFlow.VerticalScroll.Maximum;
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
            notificationComboBox.ForeColor = ColorScheme.TextOnAccent;

            captionLabel1.ForeColor = ColorScheme.TextOnAccent;
            captionLabel2.ForeColor = ColorScheme.TextOnAccent;
            captionLabel3.ForeColor = ColorScheme.TextOnAccent;
            captionLabel4.ForeColor = ColorScheme.TextOnAccent;
            heading3Label4.ForeColor = ColorScheme.TextOnAccent;

            participantsFlow.BackColor = ColorScheme.DividerColor;
            chatFlow.BackColor = ColorScheme.ChatAreaBackground;
            chatFlow.Padding = new Padding(chatFlow.Padding.Left, chatFlow.Padding.Top, chatFlow.Padding.Right, 8);

            tableLayoutPanelButtons.RowStyles[0] = new RowStyle(SizeType.AutoSize);
            tableLayoutPanel2.RowStyles[1] = new RowStyle(SizeType.AutoSize);
        }

        private bool IsMessageValid()
        {
            return !string.IsNullOrWhiteSpace(messageTextBox.Text);
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            await SendMessage();
        }

        private async Task SendMessage()
        {
            if (!IsMessageValid())
            {
                return;
            }

            try
            {
                await chatRoomService.SendMessageAsync(messageTextBox.Text.Trim(), MessageType.Text);
            }
            catch (Exception ex) when (ex is ArgumentException)
            {
                MessageBox.Show($"Что то ты слишком много кинул, скинь чуток, максимум пока только {ChatMessageConstants.MaximumMessageSize} кб");
            }
            messageTextBox.Text = string.Empty;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chatFlow_Resize(object sender, EventArgs e)
        {
            if (Width <= hideSideBarWidth)
            {
                if (!splitContainerSideBar.Panel2Collapsed)
                {
                    closeButton_Click(sender, e);
                }
                aboutButton.Visible = false;
            }
            else
            {
                aboutButton.Visible = true;
            }

            if (!isResizing)
            {
                isResizing = true;
                resizeTimer.Stop();
                resizeTimer.Start();
            }
        }

        private void PerformResize()
        {
            chatFlow.SuspendLayout();
            try
            {
                foreach (ChatMessageRow row in chatFlow.Controls)
                {
                    row.Width = chatFlow.Width - row.Margin.Horizontal;
                    var child = row.container.Controls[0];
                    if (child is ChatMessageCard card)
                    {
                        row.Height = card.ResizeOutOfPrefferedSize();
                    }
                }
            }
            finally
            {
                chatFlow.ResumeLayout();
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnsubscribeFromChatEvents();
            formCancellationTokenSource.Cancel();
            formCancellationTokenSource.Dispose();
            base.OnFormClosing(e);
        }

        private async void editButton_Click(object sender, EventArgs e)
        {
            var editForm = new RoomCreateForm(room);
            var result = editForm.ShowDialog();
            if (result == DialogResult.Abort)
            {
                Close();
            }
            else if (result == DialogResult.OK)
            {
                await chatRoomService.SendUpdateRoomRequestAsync(new RoomInfoRequestMessage()
                {
                    RoomName = editForm.Room.Name,
                    MaxParticipants = editForm.Room.MaximumParticipants,
                }, formCancellationTokenSource.Token);
            }
        }

        private async void messageTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') // Enter
            {
                if ((ModifierKeys & Keys.Shift) == 0)
                {
                    await SendMessage();
                    changeMessageBoxSize();
                    e.Handled = true;
                }
            }
        }

        private void changeMessageBoxSize()
        {
            tableLayoutPanelButtons.Height = messageTextBox.UpdateHeight() + tableLayoutPanelButtons.Margin.Vertical;
        }

        private void messageTextBox_TextChanged(object sender, EventArgs e)
        {
            changeMessageBoxSize();
        }

        private void participantsFlow_Resize(object sender, EventArgs e)
        {
            foreach (ParticipantCard card in participantsFlow.Controls.OfType<ParticipantCard>())
            {
                card.Width = participantsFlow.Width - participantsFlow.Margin.Horizontal * 2;
            }
        }
    }
}
