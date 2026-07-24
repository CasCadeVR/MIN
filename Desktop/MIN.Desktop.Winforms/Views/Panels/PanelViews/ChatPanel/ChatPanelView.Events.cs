using MIN.Chat.Events;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Stores.Contracts.Constants;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Views.Panels.SidePanelViews;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Sessions.Core.Events;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

public partial class ChatPanelView
{
    private string lastRoomName = string.Empty;
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
    }

    private async Task OnSessionReadyMessageReceived(SessionReadyMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
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
    }

    #region File related

    private async Task OnFileMetaDataMessageReceived(FileMetaDataMessageReceivedEvent eventMessage, CancellationToken ct)
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

    private async Task OnFileTransferStarted(FileTransferStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            if (eventMessage.Direction == FileTransferDirection.Upload)
            {
                var sanitizedSize = featureCollection.FileTransfer.FileHelperService.FormatFileSize(eventMessage.FileSize);
                AddStatus(new FileUploadingStatus(eventMessage.TransferId, eventMessage.FileName, eventMessage.Sender.Name, sanitizedSize));
            }
        }, this);

        await Task.CompletedTask;
    }

    private async Task OnFileTransferCompleted(FileTransferCompletedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            RemoveStatus(eventMessage.TransferId);
        }, this);

        await Task.CompletedTask;
    }

    private async Task OnFileTransferFailed(FileTransferFailedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            RemoveStatus(eventMessage.TransferId);
        }, this);

        await Task.CompletedTask;
    }

    #endregion

    #region Participant related

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
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
        if (eventMessage.RoomId != roomId)
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

    #endregion

    #region Room related

    private async Task OnRoomInfoUpdated(RoomInfoUpdatedMessageEvent eventMessage, CancellationToken ct)
    {
        if (eventMessage.RoomInfo.Id != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            if (lastRoomName != eventMessage.RoomInfo.Name)
            {
                SendSystemMessage(new SystemTextMessage
                {
                    Content = $"Хост поменял название комнаты с {lastRoomName} на {eventMessage.RoomInfo.Name}",
                }, needsToNotify: true);
                lastRoomName = eventMessage.RoomInfo.Name;
            }
            UpdateStats();
        }, null);
        await Task.CompletedTask;
    }

    private async Task OnChatHistoryUpdated(ChatHistoryUpdatedEvent eventMessage, CancellationToken ct)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            var e = eventMessage.Message;

            loadedPage = e.Page;
            room.TotalMessageCount = e.TotalCount;
            RemoveLoadMoreLabel();
            RenderMessages(e.Messages, appendOnTop: true);

            if (loadedPage * StoreConstants.MessagesPageSize < room.TotalMessageCount)
            {
                ShowLoadMoreLabel();
            }
        }, null);
        await Task.CompletedTask;
    }

    private Task OnConnectionStatusChanged(ConnectionStatusChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId == roomId && eventMessage.NeedToDisconnect)
        {
            ClearParentFormEvents();

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

    #endregion

    private async Task OnErrorOccured(ErrorOccurredEvent e, CancellationToken cancellationToken)
    {
        if (e.NeedToDisconnect)
        {
            ClearParentFormEvents();
            await DisposeAsync();
            navigationService.NavigateTo<DiscoveryPanelView>();
        }
    }
}
