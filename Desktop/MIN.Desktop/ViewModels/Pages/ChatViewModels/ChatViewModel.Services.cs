using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Enums;
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

    private async Task OnVoiceCallLeaveRequested(int subRoomId)
    {
        try
        {
            await featureCollection.Chat.ChatVoiceService.LeaveCallAsync(roomId, subRoomId, appCts.Token);
        }
        catch (DirectoryNotFoundException e)
        {
            InAppNotifier.Error(e.Message);
        }
    }

    private async Task RequestVoiceCallStateAsync()
    {
        try
        {
            await featureCollection.Chat.ChatVoiceService.RequestCallStateAsync(roomId, appCts.Token);
        }
        catch (DirectoryNotFoundException e)
        {
            InAppNotifier.Warning(e.Message);
        }
    }

    private async Task OnCancelRequested(FileMetadataMessage fileMetadata)
    {
        await featureCollection.Chat.ChatFileService.CancelFileDownloadAsync(roomId,
            fileMetadata,
            appCts.Token
        );
    }

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
    {
        try
        {
            await featureCollection.Chat.ChatSessionService.SendSessionHostRequestAsync(roomId, session, appCts.Token);
        }
        catch (DirectoryNotFoundException e)
        {
            InAppNotifier.Error(e.Message);
        }
    }

    private async Task SendVoiceCallStartMessage()
    {
        try
        {
            await featureCollection.Chat.ChatVoiceService.StartCallAsync(roomId, appCts.Token);
        }
        catch (DirectoryNotFoundException e)
        {
            InAppNotifier.Error(e.Message);
        }
    }

    [RelayCommand(CanExecute = nameof(IsMessageValid))]
    private async Task SendMessage()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SendingMessage))
            {
                await featureCollection.Chat.ChatTextService.SendMessageAsync(roomId,
                    SendingMessage.Trim(),
                    chatSideBarViewModel.PrivateChatParticipantId,
                    appCts.Token
                );
            }

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
                   fileAttachement.File.FileName,
                   fileAttachement.File.FilePath,
                   chatSideBarViewModel.PrivateChatParticipantId,
                   appCts.Token
               );
            }

            AttachedFiles.Clear();
            SendingMessage = string.Empty;
        }
        catch (Exception ex)
        {
            InAppNotifier.Error($"Не удалось отправить сообщение: {ex.Message}");
        }
    }
}
