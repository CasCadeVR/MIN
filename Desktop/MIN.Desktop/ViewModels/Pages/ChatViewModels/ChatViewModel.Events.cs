using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MIN.Chat.Events;
using MIN.Core.Events.Contracts;
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
    private HashSet<IDisposable> eventTokens = null!;

    private void SubscribeToEvents(IEventBus eventBus)
    {
        eventTokens =
        [
            eventBus.Subscribe<ChatTextMessageReceivedEvent>(OnChatTextMessageReceived),
            eventBus.Subscribe<SessionReadyMessageReceivedEvent>(OnSessionReadyMessageReceived),
            eventBus.Subscribe<FileMetaDataMessageReceivedEvent>(OnFileMetaDataMessageReceived),
            eventBus.Subscribe<FileTransferStartedEvent>(OnFileTransferStarted),
            eventBus.Subscribe<FileTransferCompletedEvent>(OnFileTransferCompleted),
            eventBus.Subscribe<FileTransferFailedEvent>(OnFileTransferFailed),
            eventBus.Subscribe<RoomInfoUpdatedMessageEvent>(OnRoomInfoUpdated),
            eventBus.Subscribe<ChatHistoryUpdatedEvent>(OnChatHistoryUpdated),
            eventBus.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined),
            eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft),
            eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccured),
            eventBus.Subscribe<ConnectionStatusChangedEvent>(OnConnectionStatusChanged),
        ];
    }

    private async Task OnChatTextMessageReceived(ChatTextMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message
        }, cancellationToken);
        NotifyIfNeeded(eventMessage.Message);
    }

    private async Task OnSessionReadyMessageReceived(SessionReadyMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        AddMessageToChatFlow(eventMessage.Message);
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
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message
        }, cancellationToken);
        NotifyIfNeeded(eventMessage.Message);
    }

    private async Task OnFileTransferStarted(FileTransferStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        if (eventMessage.Direction == FileTransferDirection.Upload)
        {
            var sanitizedSize = featureCollection.FileTransfer.FileHelperService.FormatFileSize(eventMessage.FileSize);
            AddStatus(new FileUploadingStatus(eventMessage.TransferId, eventMessage.FileName, eventMessage.Sender.Name, sanitizedSize));
        }
    }

    private async Task OnFileTransferCompleted(FileTransferCompletedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        RemoveStatus(eventMessage.TransferId);
    }

    private async Task OnFileTransferFailed(FileTransferFailedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        RemoveStatus(eventMessage.TransferId);
    }

    #endregion

    #region Participant related

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        room.AddParticipant(eventMessage.Message.Participant);

        AddMessageToChatFlow(eventMessage.Message);
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
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        var leavingParticipantId = eventMessage.Message.Participant.Id;
        room.RemoveParticipantById(leavingParticipantId);
        if (chatSideBarViewModel.PrivateChatParticipantId == leavingParticipantId)
        {
            chatSideBarViewModel.PrivateChatParticipantId = null;
        }

        AddMessageToChatFlow(eventMessage.Message);
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
        if (eventMessage.RoomInfo.Id != roomId)
        {
            return;
        }

        if (RoomName != eventMessage.RoomInfo.Name)
        {
            SendSystemMessage(new SystemTextMessage
            {
                Content = $"Хост поменял название комнаты с {RoomName} на {eventMessage.RoomInfo.Name}",
            }, needsToNotify: true);
            RoomName = eventMessage.RoomInfo.Name;
        }

        chatSideBarViewModel.UpdateStats(room);
    }

    private async Task OnChatHistoryUpdated(ChatHistoryUpdatedEvent eventMessage, CancellationToken ct)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        var e = eventMessage.Message;

        loadedPage = e.Page;
        room.TotalMessageCount = e.TotalCount;
        RemoveLoadMoreLabel();
        RenderMessages(e.Messages, appendOnTop: true);

        if (loadedPage * StoreConstants.MessagesPageSize < room.TotalMessageCount)
        {
            ShowLoadMoreLabel();
        }
    }

    private async Task OnConnectionStatusChanged(ConnectionStatusChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId == roomId && eventMessage.NeedToDisconnect)
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
