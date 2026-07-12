using System.Collections.Generic;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages.ChatViewModels;

/// <summary>
/// Методы использования сервисов для чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    //private readonly List<IDescribableStatus> currentStatuses = [];

    #region Update

    private void UpdateChatFlow()
    {
        //chatFlow.Controls.Clear();

        //var messages = room.ChatHistory;
        //RenderMessages(messages);

        //if (room.TotalMessageCount > StoreConstants.MessagesPageSize)
        //{
        //    ShowLoadMoreLabel();
        //}
    }

    private void RenderMessages(List<IMessage> messages, bool appendOnTop = false)
    {
        //for (var i = messages.Count - 1; i >= 0; i--)
        //{
        //    var index = appendOnTop ? (messages.Count - 1) - i : i;
        //    AddMessageToChatFlow(messages[index], appendOnTop, scrollToBottom: false);
        //}
    }

    #endregion

    #region Helper methods

    //private void AddStatus(IDescribableStatus status)
    //{
    //    currentStatuses.Add(status);
    //    ShowStatusRow();
    //    statusLabel.Text = string.Join(", ", currentStatuses.Select(x => x.GetDescription()));
    //}

    //private void RemoveStatus(Guid statusId)
    //{
    //    var foundStatus = currentStatuses.FirstOrDefault(x => x.Id == statusId);
    //    if (foundStatus != null)
    //    {
    //        currentStatuses.Remove(foundStatus);
    //    }

    //    if (currentStatuses.Count == 0)
    //    {
    //        HideStatusRow();
    //    }
    //    else
    //    {
    //        statusLabel.Text = string.Join(", ", currentStatuses.Select(x => x.GetDescription()));
    //    }
    //}

    //private void UploadFile(string filePath)
    //{
    //    ShowMultiFileAttachmentUploader();
    //    multiFileAttachmentUploader.OnLastFileRemoved
    //        += () => HideMultiFileAttachmentUploader();
    //    var fileAttachment = new FileAttachment(Path.GetFileName(filePath),
    //        filePath);

    //    multiFileAttachmentUploader.AddFileAttachment(fileAttachment);
    //}

    #endregion

    #region Extra rows

    //private void HideMultiFileAttachmentUploader(bool withClear = false)
    //{
    //    if (withClear)
    //    {
    //        multiFileAttachmentUploader.Clear();
    //    }

    //    var row = tableLayoutPanelButtons.GetRow(multiFileAttachmentUploader);

    //    tableLayoutPanelButtons.SuspendLayout();

    //    tableLayoutPanelButtons.RowStyles[row].Height = 0;

    //    multiFileAttachmentUploader.Visible = false;
    //    multiFileAttachmentUploader.OnLastFileRemoved = null;

    //    tableLayoutPanelButtons.ResumeLayout(true);
    //    ResizeMessageTextBox();
    //}

    //private void ShowMultiFileAttachmentUploader()
    //{
    //    var row = tableLayoutPanelButtons.GetRow(multiFileAttachmentUploader);

    //    tableLayoutPanelButtons.SuspendLayout();

    //    tableLayoutPanelButtons.RowStyles[row].Height = MultiFileAttachmentUploaderHeight;

    //    multiFileAttachmentUploader.Visible = true;

    //    tableLayoutPanelButtons.ResumeLayout(true);
    //    ResizeMessageTextBox();
    //}

    //private void HideStatusRow()
    //{
    //    var row = tableLayoutPanelButtons.GetRow(statusLabel);

    //    tableLayoutPanelButtons.SuspendLayout();

    //    tableLayoutPanelButtons.RowStyles[row].Height = 0;
    //    statusLabel.Visible = false;

    //    tableLayoutPanelButtons.ResumeLayout(true);
    //    ResizeMessageTextBox();
    //}

    //private void ShowStatusRow()
    //{
    //    var row = tableLayoutPanelButtons.GetRow(statusLabel);

    //    tableLayoutPanelButtons.SuspendLayout();

    //    tableLayoutPanelButtons.RowStyles[row].Height = StatsLabelHeight;
    //    statusLabel.Visible = true;

    //    tableLayoutPanelButtons.ResumeLayout(true);
    //    ResizeMessageTextBox();
    //}

    #endregion
}
