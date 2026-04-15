using MIN.Chat.Events;
using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Components;
using MIN.Desktop.Components.Labels;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Views.Components;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Services;

namespace MIN.Desktop
{
    /// <summary>
    /// Форма чата
    /// </summary>
    public partial class ChatForm : StyledForm
    {
        private readonly IChatService chatService;
        private readonly IRoomStore roomStore;
        private readonly IEventBus eventBus;
        private readonly INotificationService notificationService;
        private readonly ILoggerProvider logger;

        private readonly CancellationTokenSource formCts = new();
        private readonly SynchronizationContext uiContext;

        private readonly int hideSideBarWidth;
        private readonly int messageMinPadding = 4;

        private readonly System.Windows.Forms.Timer resizeTimer = new() { Interval = 150 };

        private readonly Guid roomId;
        private readonly IEndpoint endpoint;
        private readonly ParticipantInfo localParticipant;

        private bool isResizing;
        private Room? room;
        private ChatTextMessage? lastTextMessage;

        private HashSet<IDisposable> eventTokens = null!;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ChatForm"/>
        /// </summary>
        public ChatForm(
             IChatService chatService,
             IRoomStore roomStore,
             IEventBus eventBus,
             INotificationService notificationService,
             ILoggerProvider logger,
             IIdentityService identitiyService,
             Guid roomId,
             IEndpoint endpoint)
        {
            InitializeComponent();
            SendLoadingMessage();

            this.roomStore = roomStore;
            this.chatService = chatService;
            this.eventBus = eventBus;
            this.logger = logger;
            this.roomId = roomId;
            this.endpoint = endpoint;

            this.notificationService = notificationService;
            notificationService.OnNotificationClick += () =>
            {
                WindowState = FormWindowState.Normal;
                Focus();
            };
            notificationService.NotificationTurnOffClicked += () => notificationComboBox.Checked = false;

            localParticipant = new ParticipantInfo(identitiyService.SelfPartcipant);
            this.room = this.roomStore.TryGetRoom(this.roomId, out var room) ? room : null;

            uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("Must be created on UI thread");
            resizeTimer.Tick += (s, e) =>
            {
                resizeTimer.Stop();
                isResizing = false;
                PerformResize();
            };

            hideSideBarWidth = MinimumSize.Width + splitContainerSideBar.Panel2.Width;

            SubscribeToEvents();
            UpdateStats();
            UpdateChatFlow();
        }

        private void SubscribeToEvents()
        {
            eventTokens =
            [
                eventBus.Subscribe<ChatTextMessageReceivedEvent>(OnChatTextMessageReceived),
                eventBus.Subscribe<RoomStateChangedEvent>(OnRoomStateChangedEventReceived),
                eventBus.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined),
                eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft),
                eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccured),
                eventBus.Subscribe<ConnectionStatusChangedEvent>(OnConnectionStatusChanged),
            ];
        }

        private async Task OnChatTextMessageReceived(ChatTextMessageReceivedEvent eventMessage, CancellationToken ct)
        {
            if (eventMessage.RoomId != roomId)
            {
                return;
            }

            uiContext.Post(_ =>
            {
                AddMessageToChatFlow(eventMessage.Message);
                NotifyIfNeeded(eventMessage.Message.Content, eventMessage.Sender.Name);
            }, null);
            await Task.CompletedTask;
        }

        private async Task OnRoomStateChangedEventReceived(RoomStateChangedEvent eventMessage, CancellationToken cancellationToken)
        {
            if (eventMessage.Room.Id != roomId)
            {
                return;
            }

            uiContext.Post(_ =>
            {
                room = eventMessage.Room;
                UpdateStats();
                UpdateChatFlow();
            }, null);
            await Task.CompletedTask;
        }

        private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
        {
            if (eventMessage.Message.RoomId != roomId)
            {
                return;
            }

            room!.CurrentParticipants.Add(eventMessage.Message.Participant);

            uiContext.Post(_ =>
            {
                AddMessageToChatFlow(eventMessage.Message);
                NotifyIfNeeded($"Участник {eventMessage.Message.Participant.Name} зашёл в комнату");
                UpdateStats();
            }, null);
            await Task.CompletedTask;
        }

        private async Task OnParticipantLeft(ParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
        {
            if (eventMessage.Message.RoomId != roomId)
            {
                return;
            }

            room!.CurrentParticipants.Remove(eventMessage.Message.Participant);

            uiContext.Post(_ =>
            {
                AddMessageToChatFlow(eventMessage.Message);
                NotifyIfNeeded($"Участник {eventMessage.Message.Participant.Name} вышел из комнаты");
                UpdateStats();
            }, null);
            await Task.CompletedTask;
        }

        private async Task OnConnectionStatusChanged(ConnectionStatusChangedEvent eventMessage, CancellationToken cancellationToken)
        {
            if (eventMessage.RoomId == roomId && eventMessage.NeedToDisconnect)
            {
                uiContext.Post(_ =>
                {
                    if (!string.IsNullOrEmpty(eventMessage.LeavingMessage))
                    {
                        MessageBox.Show(eventMessage.LeavingMessage,
                           "Подключение разорвано",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    Close();
                }, null);
            }
            await Task.CompletedTask;
        }

        private Task OnErrorOccured(ErrorOccurredEvent e, CancellationToken cancellationToken)
        {
            uiContext.Post(_ =>
            {
                MessageBox.Show(e.ErrorMessage,
                    "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (e.NeedToDisconnect)
                {
                    Close();
                }
            }, null);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
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
            createdAt.ForeColor = ColorScheme.TextOnAccent;
            notificationComboBox.ForeColor = ColorScheme.TextOnAccent;

            captionLabel1.ForeColor = ColorScheme.TextOnAccent;
            captionLabel2.ForeColor = ColorScheme.TextOnAccent;
            captionLabel3.ForeColor = ColorScheme.TextOnAccent;
            captionLabel4.ForeColor = ColorScheme.TextOnAccent;
            heading3Label4.ForeColor = ColorScheme.TextOnAccent;
            labelCreatedAt.ForeColor = ColorScheme.TextOnAccent;

            participantsFlow.BackColor = ColorScheme.DividerColor;
            chatFlow.BackColor = ColorScheme.ChatAreaBackground;
            chatFlow.Padding = new Padding(chatFlow.Padding.Left, chatFlow.Padding.Top, chatFlow.Padding.Right, messageMinPadding);

            tableLayoutPanelButtons.RowStyles[0] = new RowStyle(SizeType.AutoSize);
            tableLayoutPanel2.RowStyles[1] = new RowStyle(SizeType.AutoSize);
        }

        private void UpdateStats()
        {
            if (room == null)
            {
                return;
            }

            Text = $"MIN - Комната {room.Name}";
            Title.Text = $"Комната {room.Name}";

            var isHost = room.HostParticipant?.Id == localParticipant.Id;
            hostName.Text = isHost ? "Ты" : room.HostParticipant?.Name ?? "Неизвестно";
            createdAt.Text = room.CreatedAt.ToShortTimeString();
            editButton.Visible = isHost;

            if (CollegePCNameParser.TryParseComputerName(endpoint is NamedPipeEndpoint npEndpoint
                    ? npEndpoint.MachineName
                    : string.Empty,
                out var roomNumber,
                out var computerNumber))
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

            if (room == null)
            {
                return;
            }

            foreach (var participant in room.CurrentParticipants)
            {
                var card = new ParticipantCard(participant, room)
                {
                    Width = participantsFlow.Width - participantsFlow.Margin.Horizontal * 2,
                };
                participantsFlow.Controls.Add(card);
            }

            participantsInfo.Text = $"{room.ParticipantCount}/{room.MaximumParticipants}";
        }

        private void UpdateChatFlow()
        {
            chatFlow.Controls.Clear();

            if (room == null)
            {
                return;
            }

            foreach (var storedMessage in room.ChatHistory)
            {
                AddMessageToChatFlow(storedMessage);
            }
        }

        private void SendLoadingMessage()
        {
            AddMessageToChatFlow(new SystemTextMessage
            {
                Content = "Загрузка...",
            });
        }

        private void NotifyIfNeeded(string content, string? senderName = null)
        {
            if (notificationComboBox.Checked && (WindowState == FormWindowState.Minimized || !ContainsFocus))
            {
                notificationService.Notify(content, room?.Name ?? "Комната", senderName);
            }
        }

        private void AddMessageToChatFlow(IMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(() => AddMessageToChatFlow(message));
                return;
            }

            chatFlow.SuspendLayout();
            try
            {
                var row = new ChatMessageRow();
                Control rowControl;

                switch (message)
                {
                    case ChatTextMessage chatTextMessage:
                        var isSelfMessage = chatTextMessage.Sender.Id == localParticipant.Id;
                        var isHostMessage = room?.HostParticipant?.Id == chatTextMessage.Sender.Id;

                        var minutesPassed = 0;
                        if (lastTextMessage != null)
                        {
                            minutesPassed = (int)(chatTextMessage.Timestamp - lastTextMessage.Timestamp).TotalMinutes;
                            minutesPassed = minutesPassed > messageMinPadding ? messageMinPadding * 2 : minutesPassed + messageMinPadding;
                        }

                        rowControl = new ChatMessageCard(chatTextMessage,
                            localParticipant,
                            isHostMessage,
                            removeHeaders: isSelfMessage || lastTextMessage?.Sender.Id == chatTextMessage.Sender.Id)
                        {
                            Anchor = isSelfMessage ? AnchorStyles.Right : AnchorStyles.Left,
                            Margin = new Padding(20, 0, 20, 0)
                        };

                        row.Margin = new Padding(row.Margin.Left, minutesPassed, row.Margin.Right, row.Margin.Bottom);
                        lastTextMessage = chatTextMessage;
                        break;

                    case ParticipantJoinedMessage joined:
                        var joinedText = $"Участник {joined.Participant.Name} зашёл в комнату";
                        rowControl = new PrimaryLabel
                        {
                            Text = joinedText,
                            Anchor = AnchorStyles.None,
                            AutoSize = true
                        };

                        row.Height = rowControl.Height;
                        break;

                    case ParticipantLeftMessage left:
                        var leftText = $"Участник {left.Participant.Name} покинул комнату";
                        rowControl = new PrimaryLabel
                        {
                            Text = leftText,
                            Anchor = AnchorStyles.None,
                            AutoSize = true
                        };

                        row.Height = rowControl.Height;
                        break;

                    case SystemTextMessage systemTextMessage:
                        rowControl = new PrimaryLabel
                        {
                            Text = systemTextMessage.Content,
                            Anchor = AnchorStyles.None,
                            AutoSize = true
                        };

                        row.Height = rowControl.Height;
                        break;

                    default:
                        return;
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

        private bool IsMessageValid() => !string.IsNullOrWhiteSpace(messageTextBox.Text);

        private async Task SendMessage()
        {
            if (!IsMessageValid())
            {
                return;
            }

            try
            {
                await chatService.SendMessageAsync(roomId, messageTextBox.Text.Trim(), localParticipant, cancellationToken: formCts.Token);
                messageTextBox.Text = string.Empty;
                changeMessageBoxSize();
            }
            catch (Exception ex)
            {
                logger.Log($"Failed to send message: {ex.Message}");
                MessageBox.Show("Не удалось отправить сообщение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (room == null)
            {
                return;
            }

            var editForm = new RoomCreateForm(room);
            var result = editForm.ShowDialog();

            if (result == DialogResult.Abort)
            {
                Close();
            }
            else if (result == DialogResult.OK)
            {
                // TODO: Исправить

                //var requestMessage = new RoomInfoRequestMessage
                //{
                //    RoomId = roomId,
                //    RoomName = editForm.RoomData.Name,
                //    MaxParticipants = editForm.RoomData.MaximumParticipants
                //};
                //await messageSender.SendAsync(requestMessage, roomId, connectionId, formCts.Token);
            }
        }

        private async void sendButton_Click(object sender, EventArgs e) => await SendMessage();

        private void disconnectButton_Click(object sender, EventArgs e) => Close();

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

        private void participantsFlow_Resize(object sender, EventArgs e)
        {
            foreach (ParticipantCard card in participantsFlow.Controls.OfType<ParticipantCard>())
            {
                card.Width = participantsFlow.Width - participantsFlow.Margin.Horizontal * 2;
            }
        }

        private void messageTextBox_TextChanged(object sender, EventArgs e) => changeMessageBoxSize();

        private void messageTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') // Enter
            {
                if ((ModifierKeys & Keys.Shift) == 0)
                {
                    _ = SendMessage();
                    changeMessageBoxSize();
                    e.Handled = true;
                }
            }
        }

        private void changeMessageBoxSize()
        {
            tableLayoutPanelButtons.Height = messageTextBox.UpdateHeight() + tableLayoutPanelButtons.Margin.Vertical;
        }

        /// <summary>
        /// <see cref="Form.OnFormClosed(FormClosedEventArgs)"/>
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (var token in eventTokens)
            {
                token.Dispose();
            }
            formCts.Cancel();
            formCts.Dispose();
            base.OnFormClosing(e);
        }
    }
}
