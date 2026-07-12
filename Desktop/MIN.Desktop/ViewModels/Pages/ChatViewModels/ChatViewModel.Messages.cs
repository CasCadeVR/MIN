using System;
using System.Linq;
using Avalonia.Collections;
using MIN.Chat.Messaging;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Stores.Contracts.Constants;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Cards.Messages;

namespace MIN.Desktop.ViewModels.Pages.ChatViewModels;

/// <summary>
/// Методы использования сервисов для чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    /// <summary>
    /// Список сообщений для отображения в UI
    /// </summary>
    public AvaloniaList<BaseChatMessageViewModel> Messages { get; } = [];

    private readonly int messageMinPadding = 4;

    private Guid? lastPrivateChatParticipantId;
    private IMessage? lastChatMessage;
    private int loadedPage = 1;
    private SystemChatMessageViewModel? loadMoreLabel;
    private int renderedMessageCount;

    private void AddMessageToChatFlow(IMessage message, bool appendOnTop = false, bool scrollToBottom = true, bool countTowardCap = true)
    {
        try
        {
            var isSelfMessage = message.SenderId == localParticipant.Id;
            var isHostMessage = room?.HostParticipant?.Id == message.SenderId;
            var isCurrentPrivate = message.RecipientId == localParticipant.Id
                || (message.SenderId == localParticipant.Id && message.RecipientId != null);

            BaseChatMessageViewModel? messageCard = null;
            switch (message)
            {
                case ChatTextMessage m:
                    messageCard = CreateTextMessageCard(m, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop);
                    break;

                //case SessionReadyMessage m:
                //    messageCard = CreateSessionMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop);
                //    break;

                //case FileMetadataMessage m:
                //    messageCard = featureCollection.FileTransfer.FileHelperService.IsFileImage(m.FileName)
                //        ? CreateChatImagePreviewMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop)
                //        : CreateFileMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop);
                //    break;

                case SystemTextMessage m:
                    messageCard = CreateSystemMessageLabel(m);
                    break;

                case IDescribable d:
                    messageCard = CreateDescribableLabel(d);
                    break;
                default:
                    return;
            }

            if (ShouldTrimExcessMessages())
            {
                ReplaceOldestWithLoadMore();
            }

            if (appendOnTop)
            {
                Messages.Add(messageCard);
            }
            else
            {
                Messages.Insert(0, messageCard);
            }

            if (countTowardCap)
            {
                renderedMessageCount++;
            }

            //if (messageCard is IResizableComponent resizableComponent)
            //{
            //    row.Height = resizableComponent.ResizeOutOfPrefferedSize() + row.Padding.Top;
            //    resizableComponent.AskParentForResize += PerformResize;
            //}
        }
        finally
        {
            //if (scrollToBottom)
            //{
            //    chatFlow.VerticalScroll.Value = chatFlow.VerticalScroll.Maximum;
            //}
        }
    }

    private void ShowLoadMoreLabel()
    {
        if (loadMoreLabel != null)
        {
            return;
        }

        loadMoreLabel = new SystemChatMessageViewModel
        {
            Text = "+ Загрузить ещё",
        };

        //loadMoreLabel.Click += OnLoadMoreClicked;

        Messages.Add(loadMoreLabel);
    }

    private void RemoveLoadMoreLabel()
    {
        if (loadMoreLabel == null)
        {
            return;
        }

        //loadMoreLabel.Click -= OnLoadMoreClicked;

        Messages.Remove(loadMoreLabel);
        loadMoreLabel.Dispose();
        loadMoreLabel = null;
    }

    private async void OnLoadMoreClicked(object? sender, EventArgs e)
    {
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(roomId);
        var memoryCount = context.Messages.GetMessageCount();

        if (memoryCount < room.TotalMessageCount)
        {
            await featureCollection.Chat.ChatRoomService.SendChatHistoryRequest(roomId, loadedPage + 1, formCts.Token);
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
        //var maxVisible = loadedPage * StoreConstants.MessagesPageSize;
        //return renderedMessageCount >= maxVisible;
        return true;
    }

    private void ReplaceOldestWithLoadMore()
    {
        //for (var i = chatFlow.Controls.Count - 1; i >= 0; i--)
        //{
        //    if (chatFlow.Controls[i] is ChatMessageRow row && row.container.Controls[0] != loadMoreLabel)
        //    {
        //        chatFlow.Controls.RemoveAt(i);
        //        renderedMessageCount--;
        //        break;
        //    }
        //}

        ShowLoadMoreLabel();
    }

    private ChatTextMessageViewModel CreateTextMessageCard(ChatTextMessage msg,
            bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var minutesPassed = CalculateTimePadding(msg.Timestamp);

        var card = new ChatTextMessageViewModel(msg, isSelf, isHost, removeHeaders);

        if (!withAppendOnTop)
        {
            InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
        }

        lastChatMessage = msg;
        return card;
    }

    //private ChatSessionMessageCard CreateSessionMessageCard(SessionReadyMessage msg, ChatMessageRow row,
    //       bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    //{
    //    var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
    //    var minutesPassed = CalculateTimePadding(msg.Timestamp);

    //    var card = new ChatSessionMessageCard(featureCollection.Sessions,
    //        featureCollection.Core.EventBus, roomId,
    //        msg, localParticipant, isHost, removeHeaders)
    //    {
    //        Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
    //        Margin = new Padding(20, 0, 20, 0),
    //    };
    //    card.OnJoinRequested += () => OnSessionJoinRequested(msg);

    //    if (!withAppendOnTop)
    //    {
    //        InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
    //    }
    //    ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
    //    row.Height = card.Height;
    //    lastChatMessage = msg;
    //    return card;
    //}

    //private ChatFileMessageCard CreateFileMessageCard(FileMetadataMessage msg, ChatMessageRow row,
    //    bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    //{
    //    var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
    //    var minutesPassed = CalculateTimePadding(msg.Timestamp);

    //    var card = new ChatFileMessageCard(featureCollection.FileTransfer,
    //        featureCollection.Core.EventBus,
    //        msg, localParticipant, isHost, removeHeaders)
    //    {
    //        Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
    //        Margin = new Padding(20, 0, 20, 0),
    //    };

    //    card.OnDownloadRequested += () => OnDownloadRequested(msg);
    //    card.OnCancelRequested += () => OnCancelRequested(msg);
    //    card.OnCardContextMenuStripClicked += () => OnShowFileClicked(msg.FilePath);

    //    if (!withAppendOnTop)
    //    {
    //        InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
    //    }
    //    ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
    //    row.Height = card.Height;
    //    lastChatMessage = msg;
    //    return card;
    //}

    //private ChatImagePreviewMessageCard CreateChatImagePreviewMessageCard(FileMetadataMessage msg, ChatMessageRow row,
    //bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    //{
    //    var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
    //    var minutesPassed = CalculateTimePadding(msg.Timestamp);

    //    var card = new ChatImagePreviewMessageCard(featureCollection.FileTransfer,
    //        featureCollection.Core.EventBus,
    //        msg, localParticipant, isHost, removeHeaders)
    //    {
    //        Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
    //        Margin = new Padding(20, 0, 20, 0),
    //    };

    //    card.OnDownloadRequested += () => OnDownloadRequested(msg);
    //    card.OnCancelRequested += () => OnCancelRequested(msg);
    //    card.OnCardContextMenuStripClicked += () => OnShowFileClicked(msg.FilePath);
    //    if (!withAppendOnTop)
    //    {
    //        InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
    //    }
    //    ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
    //    row.Height = card.Height;
    //    lastChatMessage = msg;
    //    return card;
    //}

    private static SystemChatMessageViewModel CreateSystemMessageLabel(SystemTextMessage msg)
    {
        var card = new SystemChatMessageViewModel()
        {
            Text = msg.Content,
            IsPrivate = msg.RecipientId != null,
        };

        return card;
    }

    private static SystemChatMessageViewModel CreateDescribableLabel(IDescribable describable)
    {
        var card = new SystemChatMessageViewModel()
        {
            Text = describable.GetDescription(),
        };

        return card;
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

    #endregion
}
