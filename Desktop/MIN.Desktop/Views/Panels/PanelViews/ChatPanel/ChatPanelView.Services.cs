using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

public partial class ChatPanelView
{
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
