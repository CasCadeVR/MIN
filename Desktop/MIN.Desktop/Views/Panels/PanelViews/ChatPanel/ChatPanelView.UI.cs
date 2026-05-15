using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Stores.Contracts.Constants;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Infrastructure.Events;
using MIN.Helpers.Services;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

public partial class ChatPanelView
{
    private readonly List<IDescribableStatus> currentStatuses = [];
    private const int StatsLabelHeight = 15;
    private const int MultiFileAttachmentUploaderHeight = 115;

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        splitContainerSideBar.Panel1.BackColor = ColorScheme.ChatPanelFileDropBackground;
        splitContainerSideBar.Panel2.BackColor = ColorScheme.PrimaryAccent;
        tableLayoutPanelHeader.BackColor = ColorScheme.PrimaryAccent;
        tableLayoutPanelStats.BackColor = ColorScheme.PrimaryAccent;
        notificationComboBox.BackColor = ColorScheme.PrimaryAccent;
        tableLayoutPanelButtons.BackColor = ColorScheme.DividerColor;

        participantsInfo.ForeColor = ColorScheme.TextOnAccent;
        hostName.ForeColor = ColorScheme.TextOnAccent;
        computer.ForeColor = ColorScheme.TextOnAccent;
        classroom.ForeColor = ColorScheme.TextOnAccent;
        createdAt.ForeColor = ColorScheme.TextOnAccent;
        notificationComboBox.ForeColor = ColorScheme.TextOnAccent;
        Title.ForeColor = ColorScheme.TextOnAccent;
        statusLabel.ForeColor = ColorScheme.TextPrimary;

        hostNameLabel.ForeColor = ColorScheme.TextOnAccent;
        classroomLabel.ForeColor = ColorScheme.TextOnAccent;
        computerLabel.ForeColor = ColorScheme.TextOnAccent;
        onlineLabel.ForeColor = ColorScheme.TextOnAccent;
        participantsLabel.ForeColor = ColorScheme.TextOnAccent;
        createdAtLabel.ForeColor = ColorScheme.TextOnAccent;

        participantsFlow.BackColor = ColorScheme.DividerColor;
        chatFlow.BackColor = ColorScheme.ChatAreaBackground;
        chatFlow.Padding = new Padding(chatFlow.Padding.Left, chatFlow.Padding.Top, chatFlow.Padding.Right, messageMinPadding);

        tableLayoutPanelButtons.RowStyles[0] = new RowStyle(SizeType.AutoSize);
    }

    #region Update

    private void UpdateStats()
    {
        if (room == null)
        {
            return;
        }

        Text = $"MIN - Комната {room.Name}";
        Title.Text = $"Комната {room.Name}";

        var isHost = room.HostParticipant?.Id == localParticipant.Id;
        hostName.Text = isHost ? "Ты" : room.HostParticipant?.Name ?? "Неизвестно";
        createdAt.Text = room.CreatedAt.ToShortTimeString();
        editButton.Visible = isHost;

        if (CollegePCNameParser.TryParseComputerName(endpoint is NamedPipeEndpoint npEndpoint
                ? npEndpoint.MachineName
                : string.Empty,
            out var roomNumber,
            out var computerNumber))
        {
            computer.Text = computerNumber.ToString();
            classroom.Text = roomNumber.ToString();
        }
        else
        {
            computer.Text = DesktopConstants.UndefinedPCName;
            classroom.Text = DesktopConstants.UndefinedPCName;
        }

        UpdateParticipantFlow();
    }

    private void UpdateParticipantFlow()
    {
        participantsFlow.Controls.Clear();

        if (room == null)
        {
            return;
        }

        foreach (var participant in room.CurrentParticipants)
        {
            var card = new ParticipantCard(participant,
                featureCollection.Core.EventBus,
                roomId,
                isHost: participant.Id == room.HostParticipant.Id,
                isSelf: participant.Id == localParticipant.Id)
            {
                Width = participantsFlow.Width - participantsFlow.Margin.Horizontal * 2,
            };

            card.OnCardContextMenuStripClicked += (selected, particpant) =>
            {
                privateChatParticipantId = selected ? participant.Id : null;
            };

            participantsFlow.Controls.Add(card);
        }

        participantsInfo.Text = $"{room.ParticipantCount}/{room.MaximumParticipants}";
    }

    private void UpdateChatFlow()
    {
        chatFlow.Controls.Clear();

        var messages = room.ChatHistory;
        RenderMessages(messages);

        if (room.TotalMessageCount > StoreConstants.MessagesPageSize)
        {
            ShowLoadMoreLabel();
        }
    }

    private void RenderMessages(List<IMessage> messages, bool appendOnTop = false)
    {
        for (var i = messages.Count - 1; i >= 0; i--)
        {
            var index = appendOnTop ? (messages.Count - 1) - i : i;
            AddMessageToChatFlow(messages[index], appendOnTop, scrollToBottom: false);
        }
    }

    #endregion

    #region Helper methods

    private void AddStatus(IDescribableStatus status)
    {
        currentStatuses.Add(status);
        ShowStatusRow();
        statusLabel.Text = string.Join(", ", currentStatuses.Select(x => x.GetDescription()));
    }

    private void RemoveStatus(Guid statusId)
    {
        var foundStatus = currentStatuses.FirstOrDefault(x => x.Id == statusId);
        if (foundStatus != null)
        {
            currentStatuses.Remove(foundStatus);
        }

        if (currentStatuses.Count == 0)
        {
            HideStatusRow();
        }
        else
        {
            statusLabel.Text = string.Join(", ", currentStatuses.Select(x => x.GetDescription()));
        }
    }

    private void UploadFile(string filePath)
    {
        ShowMultiFileAttachmentUploader();
        multiFileAttachmentUploader.OnLastFileRemoved
            += () => HideMultiFileAttachmentUploader();
        var fileAttachment = new FileAttachment(Path.GetFileName(filePath),
            filePath);

        multiFileAttachmentUploader.AddFileAttachment(fileAttachment);
    }

    #endregion

    #region Extra rows

    private void HideMultiFileAttachmentUploader(bool withClear = false)
    {
        if (withClear)
        {
            multiFileAttachmentUploader.Clear();
        }

        var row = tableLayoutPanelButtons.GetRow(multiFileAttachmentUploader);

        tableLayoutPanelButtons.SuspendLayout();

        tableLayoutPanelButtons.RowStyles[row].Height = 0;

        multiFileAttachmentUploader.Visible = false;
        multiFileAttachmentUploader.OnLastFileRemoved = null;

        tableLayoutPanelButtons.ResumeLayout(true);
        ResizeMessageTextBox();
    }

    private void ShowMultiFileAttachmentUploader()
    {
        var row = tableLayoutPanelButtons.GetRow(multiFileAttachmentUploader);

        tableLayoutPanelButtons.SuspendLayout();

        tableLayoutPanelButtons.RowStyles[row].Height = MultiFileAttachmentUploaderHeight;

        multiFileAttachmentUploader.Visible = true;

        tableLayoutPanelButtons.ResumeLayout(true);
        ResizeMessageTextBox();
    }

    private void HideStatusRow()
    {
        var row = tableLayoutPanelButtons.GetRow(statusLabel);

        tableLayoutPanelButtons.SuspendLayout();

        tableLayoutPanelButtons.RowStyles[row].Height = 0;
        statusLabel.Visible = false;

        tableLayoutPanelButtons.ResumeLayout(true);
        ResizeMessageTextBox();
    }

    private void ShowStatusRow()
    {
        var row = tableLayoutPanelButtons.GetRow(statusLabel);

        tableLayoutPanelButtons.SuspendLayout();

        tableLayoutPanelButtons.RowStyles[row].Height = StatsLabelHeight;
        statusLabel.Visible = true;

        tableLayoutPanelButtons.ResumeLayout(true);
        ResizeMessageTextBox();
    }

    #endregion
}
