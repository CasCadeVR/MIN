using MIN.Chat.Events;
using MIN.Chat.Messaging;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Messaging.Stateless.RoomRelated;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Components;
using MIN.Desktop.Components.Labels;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Contracts.Views.PanelViews.Interfaces;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Views.Components;
using MIN.Desktop.Views.Forms.HelperForms;
using MIN.Desktop.Views.Panels.SidePanelViews;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Services;

namespace MIN.Desktop.Views.Panels.PanelViews;

/// <summary>
/// Панель чата
/// </summary>
public partial class ChatPanelView : StyledPanelView, IPanelInitializeDepended<(Room room, Guid connectionId, IEndpoint endpoint)>, IAsyncDisposable
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly INavigationService navigationService;

    private readonly CancellationTokenSource formCts = new();

    private readonly int hideSideBarWidth;
    private readonly int messageMinPadding = 4;

    private readonly System.Windows.Forms.Timer resizeTimer = new() { Interval = 150 };

    private readonly ParticipantInfo localParticipant;
    private string lastRoomName = string.Empty;
    private Guid roomId;
    private Guid connectionId;
    private Room room = null!;
    private IEndpoint endpoint = null!;

    private bool isResizing;
    private ChatTextMessage? lastTextMessage;
    private Guid? privateChatParticipantId;

    private HashSet<IDisposable> eventTokens = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatPanelView"/>
    /// </summary>
    public ChatPanelView(IMinFeatureCollection featureCollection,
        INavigationService navigationService)
    {
        InitializeComponent();
        SendSystemMessage(new SystemTextMessage
        {
            Content = "Загрузка...",
        });

        this.featureCollection = featureCollection;
        this.navigationService = navigationService;

        featureCollection.Helper.NotificationService.OnNotificationClick += () =>
        {
            navigationService.Parent.WindowState = FormWindowState.Normal;
            Focus();
        };
        featureCollection.Helper.NotificationService.NotificationTurnOffClicked += () => notificationComboBox.Checked = false;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        resizeTimer.Tick += (s, e) =>
        {
            resizeTimer.Stop();
            isResizing = false;
            PerformResize();
        };

        hideSideBarWidth = MinimumSize.Width + splitContainerSideBar.Panel2.Width;
        localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();
    }

    /// <inheritdoc />
    /// <remarks>
    /// Room передаётся по ссылке прямо из store, так что его обновление повлияет на room ui,
    /// но придётся ещё и обновить данные
    /// </remarks>
    public void Initialize((Room room, Guid connectionId, IEndpoint endpoint) parameters)
    {
        room = parameters.room;
        lastRoomName = room.Name;
        connectionId = parameters.connectionId;
        roomId = room.Id;
        endpoint = parameters.endpoint;

        SubscribeToEvents(featureCollection.Core.EventBus);
        UpdateStats();
        UpdateChatFlow();
    }

    private void SubscribeToEvents(IEventBus eventBus)
    {
        eventTokens =
        [
            eventBus.Subscribe<ChatTextMessageReceivedEvent>(OnChatTextMessageReceived),
            eventBus.Subscribe<RoomInfoUpdatedMessageEvent>(OnRoomInfoChangedEvent),
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
            featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
            {
                RoomId = roomId,
                DescribableMessage = eventMessage.Message
            });
            NotifyIfNeeded(eventMessage.Message);
        }, null);
        await Task.CompletedTask;
    }

    private async Task OnRoomInfoChangedEvent(RoomInfoUpdatedMessageEvent eventMessage, CancellationToken ct)
    {
        if (eventMessage.Room.Id != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            if (lastRoomName != eventMessage.Room.Name)
            {
                SendSystemMessage(new SystemTextMessage
                {
                    Content = $"Хост поменял название комнаты с {lastRoomName} на {eventMessage.Room.Name}",
                }, needsToNotify: true);
                lastRoomName = eventMessage.Room.Name;
            }
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Message.RoomId != roomId)
        {
            return;
        }

        room!.AddParticipant(eventMessage.Message.Participant);

        uiContext.Post(_ =>
        {
            AddMessageToChatFlow(eventMessage.Message);
            featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
            {
                RoomId = roomId,
                DescribableMessage = eventMessage.Message
            });
            NotifyIfNeeded(eventMessage.Message);
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

        var leavingParticipantId = eventMessage.Message.Participant.Id;
        room!.RemoveParticipantById(leavingParticipantId);
        if (privateChatParticipantId == leavingParticipantId)
        {
            privateChatParticipantId = null;
        }

        uiContext.Post(_ =>
        {
            AddMessageToChatFlow(eventMessage.Message);
            featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
            {
                RoomId = roomId,
                DescribableMessage = eventMessage.Message,
            });
            NotifyIfNeeded(eventMessage.Message);
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    private Task OnConnectionStatusChanged(ConnectionStatusChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId == roomId && eventMessage.NeedToDisconnect)
        {
            uiContext.Post(async _ =>
            {
                if (!string.IsNullOrEmpty(eventMessage.LeavingMessage))
                {
                    MessageBox.Show(eventMessage.LeavingMessage,
                       "Подключение разорвано",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                await DisposeAsync();
            }, null);
            navigationService.NavigateTo<DiscoveryPanelView>();
        }
        return Task.CompletedTask;
    }

    private async Task OnErrorOccured(ErrorOccurredEvent e, CancellationToken cancellationToken)
    {
        uiContext.Post(_ =>
        {
            MessageBox.Show(e.ErrorMessage,
                "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }, null);

        if (e.NeedToDisconnect)
        {
            await DisposeAsync();
            navigationService.NavigateTo<DiscoveryPanelView>();
        }
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
            var card = new ParticipantCard(participant,
                isHost: participant.Id == room.HostParticipant.Id,
                isSelf: participant.Id == localParticipant.Id)
            {
                Width = participantsFlow.Width - participantsFlow.Margin.Horizontal * 2,
            };

            card.OnCardContextMenuStripClicked += (selected, particpant) =>
            {
                privateChatParticipantId = selected ? participant.Id : null;
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

    private void SendSystemMessage(SystemTextMessage systemMessage, bool needsToNotify = false)
    {
        AddMessageToChatFlow(systemMessage);

        if (needsToNotify)
        {
            featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
            {
                RoomId = roomId,
                DescribableMessage = systemMessage,
            });
            NotifyIfNeeded(systemMessage);
        }
    }

    private void NotifyIfNeeded(IDescribable describable)
    {
        if (notificationComboBox.Checked
            && (navigationService.Parent.WindowState == FormWindowState.Minimized || !ContainsFocus))
        {
            featureCollection.Helper.NotificationService
                .Notify(describable, room.Name);
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

                    var isCurrentPrivate = chatTextMessage.RecipientId == localParticipant.Id
                        || (chatTextMessage.SenderId == localParticipant.Id && chatTextMessage.RecipientId != null);

                    var wasLastPrivate = lastTextMessage != null && (
                        lastTextMessage.RecipientId == localParticipant.Id
                        || (lastTextMessage.SenderId == localParticipant.Id && lastTextMessage.RecipientId != null)
                    );

                    var minutesPassed = 0;
                    if (lastTextMessage != null)
                    {
                        minutesPassed = (int)(chatTextMessage.Timestamp - lastTextMessage.Timestamp).TotalMinutes;
                        minutesPassed = minutesPassed > messageMinPadding ? messageMinPadding * 2 : minutesPassed + messageMinPadding;
                    }

                    rowControl = new ChatTextMessageCard(chatTextMessage,
                        localParticipant,
                        isHostMessage,
                        removeHeaders: isSelfMessage || lastTextMessage?.SenderId == chatTextMessage.SenderId)
                    {
                        Anchor = isSelfMessage ? AnchorStyles.Right : AnchorStyles.Left,
                        Margin = new Padding(20, 0, 20, 0)
                    };

                    if (isCurrentPrivate && !wasLastPrivate)
                    {
                        var sender = room?.CurrentParticipants.First(x => x.Id == chatTextMessage.SenderId);
                        var recipient = room?.CurrentParticipants.First(x => x.Id == chatTextMessage.RecipientId);

                        SendSystemMessage(new SystemTextMessage
                        {
                            Content = recipient?.Id != localParticipant.Id
                                ? $"Это начало приватного общения с {recipient?.Name}"
                                : $"{sender?.Name} прислал вам приватное сообщение:",
                            RecipientId = localParticipant.Id,
                        });
                    }

                    if (isCurrentPrivate)
                    {
                        row.BackColor = ColorScheme.PrivateParticipantCardBackground;
                        row.Padding = new Padding(row.Padding.Left, minutesPassed, row.Padding.Right, row.Padding.Bottom);
                    }
                    else
                    {
                        row.Margin = new Padding(row.Margin.Left, minutesPassed, row.Margin.Right, row.Margin.Bottom);
                    }


                    lastTextMessage = chatTextMessage;
                    break;

                case SystemTextMessage systemTextMessage:
                    rowControl = new PrimaryLabel
                    {
                        Text = systemTextMessage.Content,
                        Anchor = AnchorStyles.None,
                        AutoSize = true
                    };

                    if (systemTextMessage.RecipientId != null)
                    {
                        row.BackColor = ColorScheme.PrivateParticipantCardBackground;
                    }

                    row.Height = rowControl.Height;
                    break;

                case IDescribable describable:
                    rowControl = new PrimaryLabel
                    {
                        Text = describable.GetDescription(),
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

            if (rowControl is ChatTextMessageCard card)
            {
                row.Height = card.ResizeOutOfPrefferedSize() + row.Padding.Top;
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
            await featureCollection.Chat.ChatService.SendMessageAsync(roomId,
                messageTextBox.Text.Trim(),
                localParticipant,
                privateChatParticipantId,
                formCts.Token
            );
            messageTextBox.Text = string.Empty;
            changeMessageBoxSize();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось отправить сообщение: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        splitContainerSideBar.Panel2.BackColor = ColorScheme.PrimaryAccent;
        tableLayoutPanelHeader.BackColor = ColorScheme.PrimaryAccent;
        tableLayoutPanelStats.BackColor = ColorScheme.PrimaryAccent;
        notificationComboBox.BackColor = ColorScheme.PrimaryAccent;

        participantsInfo.ForeColor = ColorScheme.TextOnAccent;
        hostName.ForeColor = ColorScheme.TextOnAccent;
        computer.ForeColor = ColorScheme.TextOnAccent;
        classroom.ForeColor = ColorScheme.TextOnAccent;
        createdAt.ForeColor = ColorScheme.TextOnAccent;
        notificationComboBox.ForeColor = ColorScheme.TextOnAccent;
        Title.ForeColor = ColorScheme.TextOnAccent;

        hostNameLabel.ForeColor = ColorScheme.TextOnAccent;
        classroomLabel.ForeColor = ColorScheme.TextOnAccent;
        computerLabel.ForeColor = ColorScheme.TextOnAccent;
        onlineLabel.ForeColor = ColorScheme.TextOnAccent;
        participantsLabel.ForeColor = ColorScheme.TextOnAccent;
        createdAtLabel.ForeColor = ColorScheme.TextOnAccent;

        participantsFlow.BackColor = ColorScheme.DividerColor;
        chatFlow.BackColor = ColorScheme.ChatAreaBackground;
        chatFlow.Padding = new Padding(chatFlow.Padding.Left, chatFlow.Padding.Top, chatFlow.Padding.Right, messageMinPadding);

        tableLayoutPanelButtons.RowStyles[0] = new RowStyle(SizeType.AutoSize);
    }

    private async void editButton_Click(object sender, EventArgs e)
    {
        if (room == null)
        {
            return;
        }

        var editForm = new RoomCreateForm(new RoomInfo(room));
        var result = editForm.ShowDialog();

        if (result == DialogResult.Abort)
        {
            await DisposeAsync();
            navigationService.NavigateTo<DiscoveryPanelView>();
        }
        else if (result == DialogResult.OK
            && (editForm.Room.Name != room.Name
            || editForm.Room.MaximumParticipants != room.MaximumParticipants))
        {
            await featureCollection.Core.MessageRouter.RouteAsync(new RoomInfoUpdatedMessage
            {
                Room = editForm.Room
            }, roomId, localParticipant.Id, formCts.Token);
        }
    }

    private async void sendButton_Click(object sender, EventArgs e) => await SendMessage();

    private async void disconnectButton_Click(object sender, EventArgs e)
    {
        await DisposeAsync();
        navigationService.NavigateTo<DiscoveryPanelView>();
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

    private void chatFlow_Resize(object sender, EventArgs e)
    {
        if (Width <= hideSideBarWidth)
        {
            if (!splitContainerSideBar.Panel2Collapsed)
            {
                splitContainerSideBar.Panel2Collapsed = true;
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
                if (child is ChatTextMessageCard card)
                {
                    row.Height = card.ResizeOutOfPrefferedSize() + row.Padding.Top;
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

    private async Task CleanUpAsync(Guid roomId, Guid connectionId, bool isHost)
    {
        if (isHost)
        {
            await featureCollection.Core.RoomHoster.StopHostingAsync(roomId);
            await featureCollection.Discovery.DiscoveryService.StopDiscoveryAsync(roomId);
        }
        else
        {
            await featureCollection.Core.RoomConnector.DisconnectAsync(roomId, connectionId);
        }

        await featureCollection.Core.EventBus.PublishAsync(new RoomClosedEvent() { RoomId = roomId });
        featureCollection.Core.RoomFactory.DestroyContext(roomId);
        featureCollection.Core.RoomStore.Remove(roomId);
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync()
    {
        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
        formCts.Cancel();
        formCts.Dispose();
        await CleanUpAsync(roomId, connectionId, isHost: localParticipant.Id == room.HostParticipant.Id);
    }
}
