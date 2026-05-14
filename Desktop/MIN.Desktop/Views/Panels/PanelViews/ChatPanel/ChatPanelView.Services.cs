using System.Diagnostics;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

public partial class ChatPanelView
{
    private void InitializeNotifications()
    {
        featureCollection.Helper.NotificationService.OnNotificationClick += () =>
        {
            navigationService.Parent.WindowState = FormWindowState.Normal;
            Focus();
        };
        featureCollection.Helper.NotificationService.NotificationTurnOffClicked += ()
            => notificationComboBox.Checked = false;
    }

    private void NotifyIfNeeded(IDescribable describable)
    {
        if (notificationComboBox.Checked
            && (navigationService.Parent.WindowState == FormWindowState.Minimized || !ContainsFocus))
        {
            featureCollection.Helper.NotificationService
                .Notify(describable, room.Name);
        }
    }

    private async Task OnDownloadRequested(FileMetadataMessage fileMetadata)
    {
        await featureCollection.Chat.ChatFileService.RequestFileDownloadAsync(roomId,
            fileMetadata,
            formCts.Token
        );
    }

    private async Task OnCancelRequested(FileMetadataMessage fileMetadata)
    {
        await featureCollection.Chat.ChatFileService.CancelFileDownloadAsync(roomId,
            fileMetadata,
            formCts.Token
        );
    }

    private void OnShowFileClicked(string? filePath)
    {
        if (!Path.Exists(filePath))
        {
            MessageBox.Show("Файл не нашёлся", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Process.Start("explorer.exe", $"/select,\"{filePath}\"");
    }

    private bool IsMessageValid() => !string.IsNullOrWhiteSpace(messageTextBox.Text)
        || multiFileAttachmentUploader.AttachedFiles.Any();

    private async Task SendMessage()
    {
        if (!IsMessageValid())
        {
            return;
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(messageTextBox.Text))
            {
                await featureCollection.Chat.ChatTextService.SendMessageAsync(roomId,
                    messageTextBox.Text.Trim(),
                    privateChatParticipantId,
                    formCts.Token
                );
            }

            foreach (var fileAttachement in multiFileAttachmentUploader.AttachedFiles)
            {
                if (featureCollection.FileTransfer.FileHelperService.IsFileImage(fileAttachement.FileName))
                {
                    try
                    {
                        using var img = Image.FromFile(fileAttachement.FilePath);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show($"Не удалось загрузить файл {fileAttachement.FileName}: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                await featureCollection.Chat.ChatFileService.SendFileAsync(roomId,
                   fileAttachement.FileName,
                   fileAttachement.FilePath,
                   privateChatParticipantId,
                   formCts.Token
               );
            }

            HideMultiFileAttachmentUploader(withClear: true);
            messageTextBox.Text = string.Empty;
            ResizeMessageTextBox();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось отправить сообщение: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
