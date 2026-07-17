using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using MIN.Chat.Messaging;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Stores.Contracts.Constants;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Cards.Messages;
using MIN.Desktop.ViewModels.Cards.Messages.Files;
using MIN.Desktop.ViewModels.Cards.Messages.Sessions;
using MIN.FileTransfer.Messaging;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;

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

    private readonly int messageMaxPadding = 8;

    private Guid? lastPrivateChatParticipantId;
    private IMessage? lastChatMessage;
    private int loadedPage = 1;
    private SystemChatMessageViewModel? loadMoreLabel;
    private int renderedMessageCount;

    private void AddMessageToChatFlow(IMessage message, bool appendOnTop = false, bool scrollToBottom = true, bool countTowardCap = true)
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

            case SessionReadyMessage m:
                messageCard = CreateSessionMessageCard(m, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop);
                break;

            case FileMetadataMessage m:
                messageCard = featureCollection.FileTransfer.FileHelperService.IsFileImage(m.FileName)
                    ? CreateChatImagePreviewMessageCard(m, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop)
                    : CreateFileMessageCard(m, isSelfMessage, isHostMessage, isCurrentPrivate, appendOnTop);
                break;

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
            Messages.Insert(0, messageCard);
        }
        else
        {
            Messages.Add(messageCard);
        }

        if (countTowardCap)
        {
            renderedMessageCount++;
        }

        // TODO check if its missed (didnt saw)
        //if (scrollToBottom)
        //{
        //    chatFlow.VerticalScroll.Value = chatFlow.VerticalScroll.Maximum;
        //}
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

        loadMoreLabel.OnClicked += OnLoadMoreClicked;

        Messages.Insert(0, loadMoreLabel);
    }

    private void RemoveLoadMoreLabel()
    {
        if (loadMoreLabel == null)
        {
            return;
        }

        loadMoreLabel.OnClicked -= OnLoadMoreClicked;

        Messages.Remove(loadMoreLabel);
        loadMoreLabel.Dispose();
        loadMoreLabel = null;
    }

    private async Task OnLoadMoreClicked()
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
        var maxVisible = loadedPage * StoreConstants.MessagesPageSize;
        return renderedMessageCount >= maxVisible;
    }

    private void ReplaceOldestWithLoadMore()
    {
        for (var i = 0; i < Messages.Count; i++)
        {
            if (Messages[i] != loadMoreLabel)
            {
                Messages.RemoveAt(i);
                renderedMessageCount--;
                break;
            }
        }

        ShowLoadMoreLabel();
    }

    private ChatTextMessageViewModel CreateTextMessageCard(ChatTextMessage msg,
            bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var timePadding = CalculateTimePadding(msg.Timestamp);

        var card = new ChatTextMessageViewModel(msg, timePadding, isSelf, isHost, removeHeaders, parentWindow.Clipboard);

        if (!withAppendOnTop)
        {
            InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
        }

        lastChatMessage = msg;
        return card;
    }

    private ChatFileMessageViewModel CreateFileMessageCard(FileMetadataMessage msg,
        bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var timePadding = CalculateTimePadding(msg.Timestamp);

        var card = new ChatFileMessageViewModel(featureCollection.FileTransfer,
            featureCollection.Core.EventBus, msg, timePadding,
            localParticipant, isHost, removeHeaders, parentWindow.Clipboard);

        card.OnDownloadRequested += () => OnDownloadRequested(msg);
        card.OnCancelRequested += () => OnCancelRequested(msg);

        if (!withAppendOnTop)
        {
            InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
        }

        lastChatMessage = msg;
        return card;
    }

    private ChatSessionMessageViewModel CreateSessionMessageCard(SessionReadyMessage msg,
           bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var timePadding = CalculateTimePadding(msg.Timestamp);

        var card = new ChatSessionMessageViewModel(featureCollection.Sessions,
            featureCollection.Core.EventBus, dialogService, roomId,
            msg, localParticipant, timePadding, isHost, removeHeaders);
        card.OnJoinRequested += () => OnSessionJoinRequested(msg);

        if (!withAppendOnTop)
        {
            InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
        }

        lastChatMessage = msg;
        return card;
    }

    private ChatFileImagePreviewMessageViewModel CreateChatImagePreviewMessageCard(FileMetadataMessage msg,
    bool isSelf, bool isHost, bool isCurrentPrivate, bool withAppendOnTop)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var timePadding = CalculateTimePadding(msg.Timestamp);

        var card = new ChatFileImagePreviewMessageViewModel(featureCollection.FileTransfer,
            featureCollection.Core.EventBus, msg, timePadding,
            localParticipant, isHost, removeHeaders, parentWindow.Clipboard);

        card.OnDownloadRequested += () => OnDownloadRequested(msg);
        card.OnCancelRequested += () => OnCancelRequested(msg);

        if (!withAppendOnTop)
        {
            InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate);
        }

        lastChatMessage = msg;
        return card;
    }

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


    private Thickness CalculateTimePadding(DateTime timestamp)
    {
        if (lastChatMessage == null)
        {
            return new Thickness(0, 0, 0, 0);
        }

        var minutes = (int)(timestamp - lastChatMessage.Timestamp).TotalMinutes;
        var gap = Math.Min(minutes, messageMaxPadding);
        return new Thickness(0, gap, 0, 0);
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
