using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MIN.Chat.Events;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Stores.Contracts.Constants;
using MIN.Desktop.Contracts.Models.Statuses;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Cards;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Sessions.Core.Events;
using MIN.Voice.Events;

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

        // Text
        roomScope.Subscribe<ChatTextMessageReceivedEvent>(OnChatTextMessageReceived);

        // Files
        roomScope.Subscribe<FileMetaDataMessageReceivedEvent>(OnFileMetaDataMessageReceived);
        roomScope.Subscribe<FileTransferStartedEvent>(OnFileTransferStarted);
        roomScope.Subscribe<FileTransferCompletedEvent>(OnFileTransferCompleted);
        roomScope.Subscribe<FileTransferFailedEvent>(OnFileTransferFailed);

        // Sessions
        roomScope.Subscribe<SessionReadyMessageReceivedEvent>(OnSessionReadyMessageReceived);

        // Voice calls
        roomScope.Subscribe<VoiceCallStartedEvent>(OnVoiceCallStarted);
        roomScope.Subscribe<VoiceCallEndedEvent>(OnVoiceCallEnded);
        roomScope.Subscribe<VoiceParticipantJoinedEvent>(OnVoiceParticipantJoined);
        roomScope.Subscribe<VoiceParticipantLeftEvent>(OnVoiceParticipantLeft);
        roomScope.Subscribe<VoiceCallStateReceivedEvent>(VoiceCallStateReceived);

        // Other
        roomScope.Subscribe<RoomInfoUpdatedMessageEvent>(OnRoomInfoUpdated);
        roomScope.Subscribe<ChatHistoryUpdatedEvent>(OnChatHistoryUpdated);
        roomScope.Subscribe<ChatHistoryClearedEvent>(OnChatHistoryCleared);

        roomScope.Subscribe<MessageDeletedEvent>(ChatMessageDeleted);
        roomScope.Subscribe<MessageEditedEvent>(ChatMessageEdited);
        roomScope.Subscribe<OnlineStatusChangedEvent>(OnOnlineStatusChanged);

        roomScope.Subscribe<PingMeasuredEvent>(OnPingMeasured);
        roomScope.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined);
        roomScope.Subscribe<ParticipantLeftEvent>(OnParticipantLeft);
        roomScope.Subscribe<ConnectionStatusChangedEvent>(OnConnectionStatusChanged);

        errorToken = eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccured);
    }

    private async Task PublishNewDescribable(Guid id, IDescribable describable, CancellationToken cancellationToken)
        => await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            MessageId = id,
            DescribableMessage = describable
        }, cancellationToken);

    private async Task AddToChatFlowAndNotify<T>(T message, CancellationToken cancellationToken) where T : class, IMessage, IDescribable
    {
        await AddMessageToChatFlow(message);
        await PublishNewDescribable(message.Id, message, cancellationToken);
        NotifyIfNeeded(message);
    }

    private async Task OnChatTextMessageReceived(ChatTextMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
        => await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);

    private async Task OnSessionReadyMessageReceived(SessionReadyMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
        => await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);

    #region Voice chat related

    private async Task OnVoiceCallStarted(VoiceCallStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        IsVoiceChatActive = true;
        CallDuration = TimeSpan.Zero;
        callStartedAt = DateTime.Now;
        callTimer.Start();
        activeVoiceChatSubroomId = eventMessage.Message.SubRoomId;
        IsInVoiceChat = eventMessage.Participant.Id == localParticipant.Id;
        AddToVoiceChatParticipant(eventMessage.Participant);
        await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);
    }

    private async Task OnVoiceCallEnded(VoiceCallEndedEvent eventMessage, CancellationToken cancellationToken)
    {
        VoiceChatParticipants.Clear();
        IsMuted = false;
        callTimer.Stop();
        activeVoiceChatSubroomId = null;
        IsInVoiceChat = false;
        IsVoiceChatActive = false;
    }

    private async Task VoiceCallStateReceived(VoiceCallStateReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        VoiceChatParticipants.Clear();
        callStartedAt = eventMessage.StartedAt;
        IsVoiceChatActive = eventMessage.ActiveSubRoomId != null;
        if (IsVoiceChatActive)
        {
            callTimer.Start();
        }
        activeVoiceChatSubroomId = eventMessage.ActiveSubRoomId;
        foreach (var participant in eventMessage.CallParticipants)
        {
            AddToVoiceChatParticipant(participant);
        }
        loadingTcs.SetResult();
    }

    private async Task OnVoiceParticipantJoined(VoiceParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Participant.Id == localParticipant.Id)
        {
            IsInVoiceChat = true;
        }
        AddToVoiceChatParticipant(eventMessage.Participant);
    }

    private async Task OnVoiceParticipantLeft(VoiceParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Participant.Id == localParticipant.Id)
        {
            IsMuted = false;
            IsInVoiceChat = false;
        }
        var voiceParticipant = VoiceChatParticipants.FirstOrDefault(x => x.ParticipantId == eventMessage.Participant.Id);
        if (voiceParticipant != null)
        {
            VoiceChatParticipants.Remove(voiceParticipant);
            voiceParticipant.Dispose();
        }
    }

    private void AddToVoiceChatParticipant(Participant participant)
    {
        var card = new ParticipantVoiceCardViewModel(participant, roomScope,
            localParticipant.Id == participant.Id);
        card.OnForceMuted += (forceMuted) =>
        {
            if (forceMuted)
            {
                OnUnmuteParticipantRequested(participant.Id);
            }
            else
            {
                OnMuteParticipantRequested(participant.Id);
            }
        };
        card.OnParticipantDesiredVolumeChanged += (volume) =>
        {
            OnNewDesiredVolumeRequested(participant.Id, volume);
        };
        VoiceChatParticipants.Add(card);
    }

    #endregion

    #region File related

    private async Task OnFileMetaDataMessageReceived(FileMetaDataMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
        => await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);

    private async Task OnFileTransferStarted(FileTransferStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Direction == FileTransferDirection.Upload && IsHost)
        {
            var sanitizedSize = featureCollection.FileTransfer.FileHelperService.FormatFileSize(eventMessage.FileSize);
            AddStatus(new FileUploadingStatus(eventMessage.FileMetadataId, eventMessage.FileName, eventMessage.Sender.Name, sanitizedSize));
        }
    }

    private async Task OnFileTransferCompleted(FileTransferCompletedEvent eventMessage, CancellationToken cancellationToken)
        => RemoveStatus(eventMessage.FileMetadataId);

    private async Task OnFileTransferFailed(FileTransferFailedEvent eventMessage, CancellationToken cancellationToken)
        => RemoveStatus(eventMessage.FileMetadataId);

    #endregion

    #region Participant related

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);
        chatSideBarViewModel.UpdateParticipantFlow(room.CurrentParticipants);
    }

    private async Task OnParticipantLeft(ParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        var leavingParticipantId = eventMessage.Message.Participant.Id;
        if (chatSideBarViewModel.PrivateChatParticipantId == leavingParticipantId)
        {
            chatSideBarViewModel.PrivateChatParticipantId = null;
        }

        await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);

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

    private async Task OnChatHistoryUpdated(ChatHistoryUpdatedEvent eventMessage, CancellationToken cancellationToken)
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

    private async Task OnChatHistoryCleared(ChatHistoryClearedEvent eventMessage, CancellationToken cancellationToken)
    {
        RemoveLoadMoreLabel();
        Messages.Clear();
        renderedMessageCount = 1;
        MissedMessagesCount = 0;
        loadedPage = 1;

        await SendSystemMessage(new SystemTextMessage()
        {
            Content = (eventMessage.Message as IDescribable).GetDescription()
        });
    }

    private async Task ChatMessageDeleted(MessageDeletedEvent eventMessage, CancellationToken cancellationToken)
        => RemoveMessage(eventMessage.MessageId);

    private async Task ChatMessageEdited(MessageEditedEvent eventMessage, CancellationToken cancellationToken)
        => EditMessage(eventMessage.MessageId, eventMessage.Message.Content);

    private async Task OnOnlineStatusChanged(OnlineStatusChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (currentTypingParticipants.Contains(eventMessage.Participant) && eventMessage.Status != OnlineStatus.Typing)
        {
            if (currentTypingParticipants.Remove(eventMessage.Participant) && currentTypingParticipants.Count == 0)
            {
                RemoveStatus(currentTypingParticipantsStatusId);
            }

            return;
        }

        if (eventMessage.Status != OnlineStatus.Typing)
        {
            return;
        }

        if (currentTypingParticipants.Count == 0)
        {
            currentTypingParticipantsStatusId = Guid.NewGuid();
        }

        currentTypingParticipants.Add(eventMessage.Participant);

        if (currentTypingParticipants.Count > 1)
        {
            var existing = currentStatuses.FirstOrDefault(x => x.Id == currentTypingParticipantsStatusId);
            if (existing is ParticipantTypingStatus typingStatus)
            {
                typingStatus.ParticipantNames = currentTypingParticipants.Select(x => x.Name).ToList();
            }
        }
        else if (currentTypingParticipants.Count == 1)
        {
            AddStatus(new ParticipantTypingStatus(currentTypingParticipantsStatusId, currentTypingParticipants.Select(x => x.Name).ToList()));
        }
    }

    private async Task OnPingMeasured(PingMeasuredEvent eventMessage, CancellationToken cancellationToken)
        => chatSideBarViewModel.UpdatePing(eventMessage.PingMs);

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
            ClearParentFormEvents();
            if (!string.IsNullOrEmpty(e.ErrorMessage))
            {
                NotifyIfNeeded(e.ErrorMessage);
            }
            await Disconnect();
        }
    }
}
