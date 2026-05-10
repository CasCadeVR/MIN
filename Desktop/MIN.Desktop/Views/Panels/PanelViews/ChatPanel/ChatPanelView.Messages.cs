using MIN.Chat.Messaging;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated;
using MIN.Desktop.Components;
using MIN.Desktop.Components.Labels;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Views.Components;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

public partial class ChatPanelView
{
    private readonly int messageMinPadding = 4;

    private Guid? privateChatParticipantId;
    private IMessage? lastChatMessage;

    private void AddMessageToChatFlow(IMessage message)
    {
        if (InvokeRequired)
        {
            Invoke(() => AddMessageToChatFlow(message));
            return;
        }

        chatFlow.SuspendLayout();
        try
        {
            var row = new ChatMessageRow();
            var isSelfMessage = message.SenderId == localParticipant.Id;
            var isHostMessage = room?.HostParticipant?.Id == message.SenderId;
            var isCurrentPrivate = message.RecipientId == localParticipant.Id
                || (message.SenderId == localParticipant.Id && message.RecipientId != null);
            var wasLastPrivate = lastChatMessage != null && (
                lastChatMessage.RecipientId == localParticipant.Id
                || (lastChatMessage.SenderId == localParticipant.Id && lastChatMessage.RecipientId != null));

            Control? rowControl = null;
            switch (message)
            {
                case ChatTextMessage m:
                    rowControl = CreateTextMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, wasLastPrivate);
                    break;
                case FileMetadataMessage m:
                    rowControl = featureCollection.FileTransfer.FileHelperService.IsFileImage(m.FileName)
                        ? CreateChatImagePreviewMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, wasLastPrivate)
                        : CreateFileMessageCard(m, row, isSelfMessage, isHostMessage, isCurrentPrivate, wasLastPrivate);
                    break;
                case SystemTextMessage m:
                    rowControl = CreateSystemMessageLabel(m, row);
                    break;
                case IDescribable d:
                    rowControl = CreateDescribableLabel(d, row);
                    break;
                default:
                    return;
            }

            row.Width = chatFlow.Width;
            row.container.Controls.Add(rowControl);
            chatFlow.Controls.Add(row);
            chatFlow.Controls.SetChildIndex(chatFlow.Controls[^1], 0);

            if (rowControl is IResizableComponent resizableComponent)
            {
                row.Height = resizableComponent.ResizeOutOfPrefferedSize() + row.Padding.Top;
                resizableComponent.AskParentForResize += PerformResize;
            }
        }
        finally
        {
            chatFlow.ResumeLayout(true);
            chatFlow.VerticalScroll.Value = chatFlow.VerticalScroll.Maximum;
        }
    }

    private ChatTextMessageCard CreateTextMessageCard(ChatTextMessage msg, ChatMessageRow row,
            bool isSelf, bool isHost, bool isCurrentPrivate, bool wasLastPrivate)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var minutesPassed = CalculateTimePadding(msg.Timestamp);

        var card = new ChatTextMessageCard(msg, isSelf, isHost, removeHeaders)
        {
            Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
            Margin = new Padding(20, 0, 20, 0),
        };

        InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate, wasLastPrivate);
        ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
        lastChatMessage = msg;
        return card;
    }

    private ChatFileMessageCard CreateFileMessageCard(FileMetadataMessage msg, ChatMessageRow row,
        bool isSelf, bool isHost, bool isCurrentPrivate, bool wasLastPrivate)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var minutesPassed = CalculateTimePadding(msg.Timestamp);

        var card = new ChatFileMessageCard(featureCollection.FileTransfer,
            featureCollection.Core.EventBus,
            msg, localParticipant, isHost, removeHeaders)
        {
            Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
            Margin = new Padding(20, 0, 20, 0),
        };

        card.OnDownloadRequested += () => OnDownloadRequested(msg);
        card.OnCancelRequested += () => OnCancelRequested(msg);
        card.OnCardContextMenuStripClicked += () => OnSaveAsCLicked(msg.FilePath);

        InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate, wasLastPrivate);
        ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
        row.Height = card.Height;
        lastChatMessage = msg;
        return card;
    }

    private ChatImagePreviewMessageCard CreateChatImagePreviewMessageCard(FileMetadataMessage msg, ChatMessageRow row,
    bool isSelf, bool isHost, bool isCurrentPrivate, bool wasLastPrivate)
    {
        var removeHeaders = isSelf || lastChatMessage?.SenderId == msg.SenderId;
        var minutesPassed = CalculateTimePadding(msg.Timestamp);

        var card = new ChatImagePreviewMessageCard(featureCollection.FileTransfer,
            featureCollection.Core.EventBus,
            msg, localParticipant, isHost, removeHeaders)
        {
            Anchor = isSelf ? AnchorStyles.Right : AnchorStyles.Left,
            Margin = new Padding(20, 0, 20, 0),
        };

        card.OnDownloadRequested += () => OnDownloadRequested(msg);
        card.OnCancelRequested += () => OnCancelRequested(msg);
        card.OnCardContextMenuStripClicked += () => OnSaveAsCLicked(msg.FilePath);

        InsertPrivateChatSystemMessageIfNeeded(msg.SenderId, msg.RecipientId, isCurrentPrivate, wasLastPrivate);
        ApplyMessageRowStyling(row, isCurrentPrivate, minutesPassed);
        row.Height = card.Height;
        lastChatMessage = msg;
        return card;
    }

    private static PrimaryLabel CreateSystemMessageLabel(SystemTextMessage msg, ChatMessageRow row)
    {
        if (msg.RecipientId != null)
        {
            row.BackColor = ColorScheme.PrivateParticipantCardBackground;
        }

        var label = new PrimaryLabel
        {
            Text = msg.Content,
            Anchor = AnchorStyles.None,
            AutoSize = true,
        };

        row.Height = label.Height;
        return label;
    }

    private static PrimaryLabel CreateDescribableLabel(IDescribable describable, ChatMessageRow row)
    {
        var label = new PrimaryLabel
        {
            Text = describable.GetDescription(),
            Anchor = AnchorStyles.None,
            AutoSize = true,
        };

        row.Height = label.Height;
        return label;
    }

    #region Helper methods

    private int CalculateTimePadding(DateTime messageTimestamp)
    {
        if (lastChatMessage == null)
        {
            return 0;
        }

        var minutes = (int)(messageTimestamp - lastChatMessage.Timestamp).TotalMinutes;
        return minutes > messageMinPadding ? messageMinPadding * 2 : minutes + messageMinPadding;
    }

    private void InsertPrivateChatSystemMessageIfNeeded(Guid senderId, Guid? recipientId,
        bool isCurrentPrivate, bool wasLastPrivate)
    {
        if (!isCurrentPrivate || wasLastPrivate)
        {
            return;
        }

        var sender = room?.CurrentParticipants.FirstOrDefault(x => x.Id == senderId);
        var recipient = room?.CurrentParticipants.FirstOrDefault(x => x.Id == recipientId);

        SendSystemMessage(new SystemTextMessage
        {
            Content = recipient?.Id != localParticipant.Id
                ? $"Это начало приватного общения с {recipient?.Name}"
                : $"{sender?.Name} прислал вам приватное сообщение:",
            RecipientId = localParticipant.Id,
        });
    }

    private static void ApplyMessageRowStyling(ChatMessageRow row, bool isCurrentPrivate, int minutesPassed)
    {
        if (isCurrentPrivate)
        {
            row.BackColor = ColorScheme.PrivateParticipantCardBackground;
            row.Padding = new Padding(row.Padding.Left, minutesPassed, row.Padding.Right, row.Padding.Bottom);
        }
        else
        {
            row.Margin = new Padding(row.Margin.Left, minutesPassed, row.Margin.Right, row.Margin.Bottom);
        }
    }

    #endregion
}
