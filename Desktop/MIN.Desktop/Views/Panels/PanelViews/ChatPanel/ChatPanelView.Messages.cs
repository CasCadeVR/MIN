using MIN.Chat.Messaging;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Messaging.Stateless.RoomRelated.History;
using MIN.Core.Stores.Contracts.Constants;
using MIN.Desktop.Components;
using MIN.Desktop.Components.Labels;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Views.Components;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

public partial class ChatPanelView
{
    private readonly int messageMinPadding = 4;

    private Guid? lastPrivateChatParticipantId;
    private Guid? privateChatParticipantId;
    private IMessage? lastChatMessage;
    private int loadedPage = 1;
    private PrimaryLabel? loadMoreLabel;
    private int renderedMessageCount;

    private void AddMessageToChatFlow(IMessage message, bool appendOnTop = false, bool scrollToBottom = true, bool countTowardCap = true)
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
            var isSelfMessage = message.SenderId == localParticipant.Id;
            var isHostMessage = room?.HostParticipant?.Id == message.SenderId;
            var isCurrentPrivate = message.RecipientId == localParticipant.Id
                || (message.SenderId == localParticipant.Id && message.RecipientId != null);

            Control? rowControl = null;
            switch (message)
            {
                case ChatTextMessage m:
                    rowControl = CreateTextMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop);
                    break;
                case FileMetadataMessage m:
                    rowControl = featureCollection.FileTransfer.FileHelperService.IsFileImage(m.FileName)
                        ? CreateChatImagePreviewMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop)
                        : CreateFileMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop);
                    break;
                case SystemTextMessage m:
                    rowControl = CreateSystemMessageLabel(m, row);
                    break;
                case IDescribable d:
                    rowControl = CreateDescribableLabel(d, row);
                    break;
                default:
                    return;
            }

            row.Width = chatFlow.Width;
            row.container.Controls.Add(rowControl);

            if (ShouldTrimExcessMessages())
            {
                ReplaceOldestWithLoadMore();
            }

            chatFlow.Controls.Add(row);

            if (!appendOnTop)
            {
                chatFlow.Controls.SetChildIndex(chatFlow.Controls[^1], 0);
            }

            if (countTowardCap)
            {
                renderedMessageCount++;
            }

            if (rowControl is IResizableComponent resizableComponent)
            {
                row.Height = resizableComponent.ResizeOutOfPrefferedSize() + row.Padding.Top;
                resizableComponent.AskParentForResize += PerformResize;
            }
        }
        finally
        {
            chatFlow.ResumeLayout(true);
            if (scrollToBottom)
            {
                chatFlow.VerticalScroll.Value = chatFlow.VerticalScroll.Maximum;
            }
        }
    }

    private void ShowLoadMoreLabel()
    {
        if (loadMoreLabel != null)
        {
            return;
        }

        var row = new ChatMessageRow();

        loadMoreLabel = new PrimaryLabel
        {
            Text = "+ Загрузить ещё",
            Anchor = AnchorStyles.None,
            AutoSize = true,
            Cursor = Cursors.Hand,
        };

        loadMoreLabel.Click += OnLoadMoreClicked;

        row.Height = loadMoreLabel.Height;
        row.Width = chatFlow.Width;
        row.container.Controls.Add(loadMoreLabel);

        chatFlow.Controls.Add(row);
    }

    private void RemoveLoadMoreLabel()
    {
        if (loadMoreLabel == null)
        {
            return;
        }

        loadMoreLabel.Click -= OnLoadMoreClicked;
        chatFlow.Controls.Remove(loadMoreLabel.Parent?.Parent);
        loadMoreLabel.Dispose();
        loadMoreLabel = null;
    }

    async void OnLoadMoreClicked(object? sender, EventArgs e)
    {
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(roomId);
        var memoryCount = context.Messages.GetMessageCount();

        var messageRouter = featureCollection.Core.MessageRouter;

        if (memoryCount < room.TotalMessageCount)
        {
            var request = new ChatHistoryRequestMessage
            {
                RoomId = roomId,
                Page = loadedPage + 1,
                PageSize = StoreConstants.MessagesPageSize,
            };

            await messageRouter.RouteAsync(request,
                roomId,
                localParticipant.Id,
                formCts.Token);
        }
        else
        {
            var pageMessages = context.Messages
                .GetRecentHistory(loadedPage + 1, StoreConstants.MessagesPageSize)
                .ToList();

            loadedPage++;
            RemoveLoadMoreLabel();
            RenderMessages(pageMessages, appendOnTop: true);

            if (loadedPage * StoreConstants.MessagesPageSize < memoryCount)
            {
                ShowLoadMoreLabel();
            }
        }
    }

    private bool ShouldTrimExcessMessages()
    {
        var maxVisible = loadedPage * StoreConstants.MessagesPageSize;
        return renderedMessageCount >= maxVisible;
    }

    private void ReplaceOldestWithLoadMore()
    {
        for (var i = chatFlow.Controls.Count - 1; i >= 0; i--)
        {
            if (chatFlow.Controls[i] is ChatMessageRow row && row.container.Controls[0] != loadMoreLabel)
            {
                chatFlow.Controls.RemoveAt(i);
                renderedMessageCount--;
                break;
            }
        }

        ShowLoadMoreLabel();
    }

    private ChatTextMessageCard CreateTextMessageCard(ChatTextMessage msg, ChatMessageRow row,
            bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var minutesPassed = CalculateTimePadding(msg.Timestamp);

        var card = new ChatTextMessageCard(msg, isSelf, isHost, removeHeaders)
        {
            Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
            Margin = new Padding(20, 0, 20, 0),
        };

        if (!withAppendOnTop)
        {
            InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
        }
        ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
        lastChatMessage = msg;
        return card;
    }

    private ChatFileMessageCard CreateFileMessageCard(FileMetadataMessage msg, ChatMessageRow row,
        bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var minutesPassed = CalculateTimePadding(msg.Timestamp);

        var card = new ChatFileMessageCard(featureCollection.FileTransfer,
            featureCollection.Core.EventBus,
            msg, localParticipant, isHost, removeHeaders)
        {
            Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
            Margin = new Padding(20, 0, 20, 0),
        };

        card.OnDownloadRequested += () => OnDownloadRequested(msg);
        card.OnCancelRequested += () => OnCancelRequested(msg);
        card.OnCardContextMenuStripClicked += () => OnShowFileClicked(msg.FilePath);

        if (!withAppendOnTop)
        {
            InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
        }
        ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
        row.Height = card.Height;
        lastChatMessage = msg;
        return card;
    }

    private ChatImagePreviewMessageCard CreateChatImagePreviewMessageCard(FileMetadataMessage msg, ChatMessageRow row,
    bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var minutesPassed = CalculateTimePadding(msg.Timestamp);

        var card = new ChatImagePreviewMessageCard(featureCollection.FileTransfer,
            featureCollection.Core.EventBus,
            msg, localParticipant, isHost, removeHeaders)
        {
            Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
            Margin = new Padding(20, 0, 20, 0),
        };

        card.OnDownloadRequested += () => OnDownloadRequested(msg);
        card.OnCancelRequested += () => OnCancelRequested(msg);
        card.OnCardContextMenuStripClicked += () => OnShowFileClicked(msg.FilePath);
        if (!withAppendOnTop)
        {
            InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
        }
        ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
        row.Height = card.Height;
        lastChatMessage = msg;
        return card;
    }

    private static PrimaryLabel CreateSystemMessageLabel(SystemTextMessage msg, ChatMessageRow row)
    {
        if (msg.RecipientId != null)
        {
            row.BackColor = ColorScheme.PrivateParticipantCardBackground;
        }

        var label = new PrimaryLabel
        {
            Text = msg.Content,
            Anchor = AnchorStyles.None,
            AutoSize = true,
        };

        row.Height = label.Height;
        return label;
    }

    private static PrimaryLabel CreateDescribableLabel(IDescribable describable, ChatMessageRow row)
    {
        var label = new PrimaryLabel
        {
            Text = describable.GetDescription(),
            Anchor = AnchorStyles.None,
            AutoSize = true,
        };

        row.Height = label.Height;
        return label;
    }

    #region Helper methods

    private int CalculateTimePadding(DateTime messageTimestamp)
    {
        if (lastChatMessage == null)
        {
            return 0;
        }

        var minutes = (int)(messageTimestamp - lastChatMessage.Timestamp).TotalMinutes;
        return minutes > messageMinPadding ? messageMinPadding * 2 : minutes + messageMinPadding;
    }

    private void SendSystemMessage(SystemTextMessage systemMessage, bool needsToNotify = false,
        bool countTowardCap = false)
    {
        AddMessageToChatFlow(systemMessage, scrollToBottom: true, countTowardCap: countTowardCap);

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

    private void InsertPrivateChatSystemMessageIfNeeded(Guid senderId, Guid? recipientId,
        bool isCurrentPrivate)
    {
        if (!isCurrentPrivate)
        {
            return;
        }

        var otherParticipantId = senderId == localParticipant.Id ? recipientId : senderId;
        if (lastPrivateChatParticipantId != null && otherParticipantId == lastPrivateChatParticipantId)
        {
            return;
        }

        lastPrivateChatParticipantId = otherParticipantId;

        var sender = room?.CurrentParticipants.FirstOrDefault(x => x.Id == senderId);
        var recipient = room?.CurrentParticipants.FirstOrDefault(x => x.Id == recipientId);

        SendSystemMessage(new SystemTextMessage
        {
            Content = recipient?.Id != localParticipant.Id
                ? $"Это начало приватного общения с {recipient?.Name}"
                : $"{sender?.Name} прислал вам приватное сообщение:",
            RecipientId = localParticipant.Id,
        });
    }

    private static void ApplyMessageRowStyling(ChatMessageRow row, bool isCurrentPrivate, int minutesPassed)
    {
        if (isCurrentPrivate)
        {
            row.BackColor = ColorScheme.PrivateParticipantCardBackground;
            row.Padding = new Padding(row.Padding.Left, minutesPassed, row.Padding.Right, row.Padding.Bottom);
        }
        else
        {
            row.Margin = new Padding(row.Margin.Left, minutesPassed, row.Margin.Right, row.Margin.Bottom);
        }
    }

    #endregion
}
