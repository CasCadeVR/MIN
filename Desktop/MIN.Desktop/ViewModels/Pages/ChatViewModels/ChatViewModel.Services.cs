using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Windows;
using MIN.FileTransfer.Messaging;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Desktop.ViewModels.Pages.ChatViewModels;

/// <summary>
/// Методы использования сервисов для чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    private Window parentWindow = null!;

    /// <summary>
    /// Превью ответа на вопрос (просто показать в строчке описание сообщения)
    /// </summary>
    [ObservableProperty]
    public partial string? ReplyToPreview { get; set; }

    /// <summary>
    /// Отправляемое сообщение в textBox
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    public partial string SendingMessage { get; set; } = string.Empty;

    private void InitializeNotifications()
    {
        parentWindow = MainWindowViewModel.GetWindow()!;

        featureCollection.Helper.NotificationService.OnNotificationClick += () =>
        {
            parentWindow.WindowState = WindowState.Normal;
            parentWindow.Focus();
        };

        featureCollection.Helper.NotificationService.NotificationTurnOffClicked += ()
            => room.LocalRoomSettings.NotificationsEnabled = false;
    }

    private void NotifyIfNeeded(IDescribable describable)
    {
        if (room.LocalRoomSettings.NotificationsEnabled
            && (parentWindow.WindowState == WindowState.Minimized || !isParentWindowActive))
        {
            featureCollection.Helper.NotificationService.Notify(describable, room.Name);
        }
    }

    private void NotifyIfNeeded(string message)
    {
        if (room.LocalRoomSettings.NotificationsEnabled
            && (parentWindow.WindowState == WindowState.Minimized || !isParentWindowActive))
        {
            featureCollection.Helper.NotificationService.Notify(message, room.Name);
        }
    }

    private void SetReplyTo(IMessage message)
    {
        ReplyToPreview = (message as IDescribable)?.GetDescription();
    }

    [RelayCommand]
    private void ResetReplyTo()
    {
        ReplyToPreview = null;
    }

    private async Task OnDownloadRequested(FileMetadataMessage fileMetadata)
    {
        await featureCollection.Chat.ChatFileService.RequestFileDownloadAsync(roomId,
            fileMetadata,
            appCts.Token
        );
    }

    private async Task OnSessionJoinRequested(SessionReadyMessage sessionReadyMessage)
    {
        try
        {
            await featureCollection.Chat.ChatSessionService.SendSessionJoinRequest(roomId,
                sessionReadyMessage,
                appCts.Token
            );
        }
        catch (DirectoryNotFoundException e)
        {
            InAppNotifier.Error(e.Message);
        }
    }

    private async Task OnVoiceCallJoinRequested(int subRoomId)
    {
        try
        {
            await featureCollection.Chat.ChatVoiceService.JoinCallAsync(roomId, subRoomId, appCts.Token);
        }
        catch (DirectoryNotFoundException e)
        {
            InAppNotifier.Error(e.Message);
        }
    }

    private async Task OnMuteSelfRequested()
    {
        if (activeVoiceChatSubroomId != null)
        {
            await featureCollection.Voice.MuteService.MuteSelf(roomId, activeVoiceChatSubroomId.Value, appCts.Token);
        }
    }

    private async Task OnUnmuteSelfRequested()
    {
        if (activeVoiceChatSubroomId != null)
        {
            await featureCollection.Voice.MuteService.UnmuteSelf(roomId, activeVoiceChatSubroomId.Value, appCts.Token);
        }
    }

    private void OnMuteParticipantRequested(Guid participantId)
        => featureCollection.Voice.MuteService.MuteParticipant(participantId);

    private void OnUnmuteParticipantRequested(Guid participantId)
        => featureCollection.Voice.MuteService.UnmuteParticipant(participantId);

    private void OnNewDesiredVolumeRequested(Guid participantId, int volume)
        => featureCollection.Voice.VoicePlayback.ChangeParticipantVolume(participantId, volume);

    private async Task OnVoiceCallLeaveRequested(int subRoomId)
        => await featureCollection.Chat.ChatVoiceService.LeaveCallAsync(roomId, subRoomId, appCts.Token);

    private async Task RequestVoiceCallStateAsync()
        => await featureCollection.Chat.ChatVoiceService.RequestCallStateAsync(roomId, appCts.Token);

    private async Task OnCancelRequested(FileMetadataMessage fileMetadata)
        => await featureCollection.Chat.ChatFileService.CancelFileDownloadAsync(roomId,
            fileMetadata,
            appCts.Token
        );

    private bool IsMessageValid() => !string.IsNullOrWhiteSpace(SendingMessage) || SomeFilesAttached;

    private async Task SendSelfStatusChangedMessage(OnlineStatus newStatus)
    {
#if DEBUG
        return;
#else
        try
        {
            await featureCollection.Chat.ChatStatusService.SendSelfOnlineStatusChangedAsync(roomId,
                newStatus,
                appCts.Token
            );
        }
        catch { }
#endif
    }

    private async Task SendSessionStartMessage(Session session)
        => await featureCollection.Chat.ChatSessionService.SendSessionHostRequestAsync(roomId, session, appCts.Token);

    private async Task SendVoiceCallStartMessage()
        => await featureCollection.Chat.ChatVoiceService.StartCallAsync(roomId, appCts.Token);

    private async Task OnMessageDeleteRequested(Guid id)
        => await featureCollection.Chat.ChatMessageService.DeleteMessageAsync(roomId, id, appCts.Token);

    private async Task OnMessageEditRequested(Guid id, string newContent)
        => await featureCollection.Chat.ChatMessageService.EditTextMessageAsync(roomId, id, newContent, appCts.Token);

    [RelayCommand(CanExecute = nameof(IsMessageValid))]
    private async Task SendMessage()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SendingMessage) && AttachedFiles.Count == 0)
            {
                await featureCollection.Chat.ChatTextService.SendTextMessageAsync(roomId,
                    SendingMessage.Trim(),
                    chatSideBarViewModel.PrivateChatParticipantId,
                    ReplyToPreview,
                    appCts.Token
                );
            }

            var lastFile = AttachedFiles.LastOrDefault();

            foreach (var fileAttachement in AttachedFiles)
            {
                if (featureCollection.FileTransfer.FileHelperService.IsFileImage(fileAttachement.File.FileName))
                {
                    try
                    {
                        using var bitmap = new Bitmap(fileAttachement.File.FilePath);
                    }
                    catch
                    {
                        try
                        {
                            using var bitmap = ImageHelper.SvgToBitmap(fileAttachement.File.FilePath);
                        }
                        catch (ArgumentException ex)
                        {
                            InAppNotifier.Error($"Не удалось загрузить файл {fileAttachement.File.FileName}: {ex.Message}");
                            return;
                        }
                    }
                }

                await featureCollection.Chat.ChatFileService.SendFileAsync(roomId,
                   fileAttachement == lastFile ? SendingMessage : string.Empty,
                   fileAttachement.File.FileName,
                   fileAttachement.File.FilePath,
                   chatSideBarViewModel.PrivateChatParticipantId,
                   ReplyToPreview,
                   appCts.Token
               );
            }

            AttachedFiles.Clear();
            SendingMessage = string.Empty;
            ReplyToPreview = null;
        }
        catch (Exception ex)
        {
            InAppNotifier.Error($"Не удалось отправить сообщение: {ex.Message}");
        }
    }
}
