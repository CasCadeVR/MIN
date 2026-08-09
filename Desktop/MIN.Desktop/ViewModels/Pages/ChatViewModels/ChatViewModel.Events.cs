using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MIN.Chat.Events;
using MIN.Common.Core.Contracts.Interfaces;
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
        roomScope.Subscribe<ChatTextMessageReceivedEvent>(OnChatTextMessageReceived);
        roomScope.Subscribe<FileMetaDataMessageReceivedEvent>(OnFileMetaDataMessageReceived);
        roomScope.Subscribe<FileTransferStartedEvent>(OnFileTransferStarted);
        roomScope.Subscribe<FileTransferCompletedEvent>(OnFileTransferCompleted);
        roomScope.Subscribe<FileTransferFailedEvent>(OnFileTransferFailed);
        roomScope.Subscribe<SessionReadyMessageReceivedEvent>(OnSessionReadyMessageReceived);

        roomScope.Subscribe<VoiceCallStartedEvent>(OnVoiceCallStarted);
        roomScope.Subscribe<VoiceCallEndedEvent>(OnVoiceCallEnded);
        roomScope.Subscribe<VoiceParticipantJoinedEvent>(OnVoiceParticipantJoined);
        roomScope.Subscribe<VoiceParticipantLeftEvent>(OnVoiceParticipantLeft);
        roomScope.Subscribe<VoiceCallStateReceivedEvent>(VoiceCallStateReceived);

        roomScope.Subscribe<RoomInfoUpdatedMessageEvent>(OnRoomInfoUpdated);
        roomScope.Subscribe<ChatHistoryUpdatedEvent>(OnChatHistoryUpdated);
        roomScope.Subscribe<PingMeasuredEvent>(OnPingMeasured);
        roomScope.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined);
        roomScope.Subscribe<ParticipantLeftEvent>(OnParticipantLeft);
        roomScope.Subscribe<ConnectionStatusChangedEvent>(OnConnectionStatusChanged);

        errorToken = eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccured);
    }

    private async Task PublishNewDescribable(IDescribable describable, CancellationToken cancellationToken)
        => await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = describable
        }, cancellationToken);

    private async Task AddToChatFlowAndNotify<T>(T message, CancellationToken cancellationToken) where T : class, IMessage, IDescribable
    {
        await AddMessageToChatFlow(message);
        await PublishNewDescribable(message, cancellationToken);
        NotifyIfNeeded(message);
    }

    private async Task OnChatTextMessageReceived(ChatTextMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);
    }

    private async Task OnSessionReadyMessageReceived(SessionReadyMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);
    }

    #region Voice chat related

    private async Task OnVoiceCallStarted(VoiceCallStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        IsVoiceChatActive = true;
        activeVoiceChatSubroomId = eventMessage.Message.SubRoomId;
        IsInVoiceChat = eventMessage.Participant.Id == localParticipant.Id;
        VoiceChatParticipants.Add(new ParticipantVoiceCardViewModel(eventMessage.Participant, roomScope));
        await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);
    }

    private async Task OnVoiceCallEnded(VoiceCallEndedEvent eventMessage, CancellationToken cancellationToken)
    {
        VoiceChatParticipants.Clear();
        activeVoiceChatSubroomId = null;
        IsInVoiceChat = false;
        IsVoiceChatActive = false;
    }

    private async Task VoiceCallStateReceived(VoiceCallStateReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        VoiceChatParticipants.Clear();
        IsVoiceChatActive = eventMessage.ActiveSubRoomId != null;
        activeVoiceChatSubroomId = eventMessage.ActiveSubRoomId;
        foreach (var participant in eventMessage.CallParticipants)
        {
            VoiceChatParticipants.Add(new ParticipantVoiceCardViewModel(participant, roomScope));
        }
        loadingTcs.SetResult();
    }

    private async Task OnVoiceParticipantJoined(VoiceParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        IsInVoiceChat = eventMessage.Participant.Id == localParticipant.Id;
        VoiceChatParticipants.Add(new ParticipantVoiceCardViewModel(eventMessage.Participant, roomScope));
    }

    private async Task OnVoiceParticipantLeft(VoiceParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Participant.Id == localParticipant.Id)
        {
            IsInVoiceChat = false;
        }
        var voiceParticipant = VoiceChatParticipants.FirstOrDefault(x => x.ParticipantId == eventMessage.Participant.Id);
        if (voiceParticipant != null)
        {
            VoiceChatParticipants.Remove(voiceParticipant);
            voiceParticipant.Dispose();
        }
    }

    #endregion

    #region File related

    private async Task OnFileMetaDataMessageReceived(FileMetaDataMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        await AddToChatFlowAndNotify(eventMessage.Message, cancellationToken);
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
            ClearParentFormEvents();
            if (!string.IsNullOrEmpty(e.ErrorMessage))
            {
                NotifyIfNeeded(e.ErrorMessage);
                InAppNotifier.Info(e.ErrorMessage);
            }
            await Disconnect();
        }
    }
}
