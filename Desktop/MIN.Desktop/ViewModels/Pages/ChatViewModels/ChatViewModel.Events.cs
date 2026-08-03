using System;
using System.Threading;
using System.Threading.Tasks;
using MIN.Chat.Events;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Stores.Contracts.Constants;
using MIN.Desktop.Contracts.Models.Statuses;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Sessions.Core.Events;

namespace MIN.Desktop.ViewModels.Pages.ChatViewModels;

/// <summary>
/// Модель чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    private IEventScope roomScope = null!;
    private IDisposable errorToken = null!;

    private void SubscribeToEvents(IEventBus eventBus)
    {
        roomScope = eventBus.CreateScope(roomId);
        roomScope.Subscribe<ChatTextMessageReceivedEvent>(OnChatTextMessageReceived);
        roomScope.Subscribe<SessionReadyMessageReceivedEvent>(OnSessionReadyMessageReceived);
        roomScope.Subscribe<FileMetaDataMessageReceivedEvent>(OnFileMetaDataMessageReceived);
        roomScope.Subscribe<FileTransferStartedEvent>(OnFileTransferStarted);
        roomScope.Subscribe<FileTransferCompletedEvent>(OnFileTransferCompleted);
        roomScope.Subscribe<FileTransferFailedEvent>(OnFileTransferFailed);
        roomScope.Subscribe<RoomInfoUpdatedMessageEvent>(OnRoomInfoUpdated);
        roomScope.Subscribe<ChatHistoryUpdatedEvent>(OnChatHistoryUpdated);
        roomScope.Subscribe<PingMeasuredEvent>(OnPingMeasured);
        roomScope.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined);
        roomScope.Subscribe<ParticipantLeftEvent>(OnParticipantLeft);
        roomScope.Subscribe<ConnectionStatusChangedEvent>(OnConnectionStatusChanged);

        errorToken = eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccured);
    }

    private async Task OnChatTextMessageReceived(ChatTextMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        await AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message
        }, cancellationToken);
        NotifyIfNeeded(eventMessage.Message);
    }

    private async Task OnSessionReadyMessageReceived(SessionReadyMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        await AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message
        }, cancellationToken);
        NotifyIfNeeded(eventMessage.Message);
    }

    #region File related

    private async Task OnFileMetaDataMessageReceived(FileMetaDataMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        await AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message
        }, cancellationToken);
        NotifyIfNeeded(eventMessage.Message);
    }

    private async Task OnFileTransferStarted(FileTransferStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Direction == FileTransferDirection.Upload)
        {
            var sanitizedSize = featureCollection.FileTransfer.FileHelperService.FormatFileSize(eventMessage.FileSize);
            AddStatus(new FileUploadingStatus(eventMessage.TransferId, eventMessage.FileName, eventMessage.Sender.Name, sanitizedSize));
        }
    }

    private async Task OnFileTransferCompleted(FileTransferCompletedEvent eventMessage, CancellationToken cancellationToken)
    {
        RemoveStatus(eventMessage.TransferId);
    }

    private async Task OnFileTransferFailed(FileTransferFailedEvent eventMessage, CancellationToken cancellationToken)
    {
        RemoveStatus(eventMessage.TransferId);
    }

    #endregion

    #region Participant related

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        room.AddParticipant(eventMessage.Message.Participant);

        await AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message
        }, cancellationToken);
        NotifyIfNeeded(eventMessage.Message);

        chatSideBarViewModel.UpdateParticipantFlow(room.CurrentParticipants);
    }

    private async Task OnParticipantLeft(ParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        var leavingParticipantId = eventMessage.Message.Participant.Id;
        room.RemoveParticipantById(leavingParticipantId);
        if (chatSideBarViewModel.PrivateChatParticipantId == leavingParticipantId)
        {
            chatSideBarViewModel.PrivateChatParticipantId = null;
        }

        await AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message,
        }, cancellationToken);
        NotifyIfNeeded(eventMessage.Message);

        chatSideBarViewModel.UpdateParticipantFlow(room.CurrentParticipants);
    }

    #endregion

    #region Room related

    private async Task OnRoomInfoUpdated(RoomInfoUpdatedMessageEvent eventMessage, CancellationToken ct)
    {
        if (RoomName != eventMessage.RoomInfo.Name)
        {
            await SendSystemMessage(new SystemTextMessage
            {
                Content = $"Хост поменял название комнаты с {RoomName} на {eventMessage.RoomInfo.Name}",
            }, needsToNotify: true);
            RoomName = eventMessage.RoomInfo.Name;
        }

        chatSideBarViewModel.UpdateStats(room);
    }

    private async Task OnChatHistoryUpdated(ChatHistoryUpdatedEvent eventMessage, CancellationToken ct)
    {
        var e = eventMessage.Message;

        loadedPage = e.Page;
        room.TotalMessageCount = e.TotalCount;
        RemoveLoadMoreLabel();
        await RenderMessages(e.Messages, appendOnTop: true);

        if (loadedPage * StoreConstants.MessagesPageSize < room.TotalMessageCount)
        {
            ShowLoadMoreLabel();
        }
    }

    private async Task OnPingMeasured(PingMeasuredEvent eventMessage, CancellationToken cancellationToken)
    {
        chatSideBarViewModel.UpdatePing(eventMessage.PingMs);
    }

    private async Task OnConnectionStatusChanged(ConnectionStatusChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.NeedToDisconnect)
        {
            ClearParentFormEvents();

            if (!string.IsNullOrEmpty(eventMessage.LeavingMessage))
            {
                NotifyIfNeeded(eventMessage.LeavingMessage);
                InAppNotifier.Info(eventMessage.LeavingMessage);
            }
            await Disconnect();
        }
    }

    #endregion

    private async Task OnErrorOccured(ErrorOccurredEvent e, CancellationToken cancellationToken)
    {
        if (e.NeedToDisconnect)
        {
            NotifyIfNeeded(e.ErrorMessage);
            ClearParentFormEvents();
            await Disconnect();
        }
    }
}
