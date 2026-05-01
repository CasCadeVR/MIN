using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Schemes;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения, представляющая файл от пользователя
/// </summary>
public partial class ChatFileMessageCard : UserControl
{
    private readonly FileMetadataMessage fileMetadataMessage;
    private readonly ParticipantInfo localParticipant;
    private readonly bool hostMessage;
    private readonly bool removeHeaders;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomDiscoveryCard"/>
    /// </summary>
    public ChatFileMessageCard(FileMetadataMessage fileMetadataMessage,
        ParticipantInfo localParticipant,
        bool hostMessage,
        bool removeHeaders)
    {
        InitializeComponent();
        this.fileMetadataMessage = fileMetadataMessage;
        this.localParticipant = localParticipant;
        this.hostMessage = hostMessage;
        this.removeHeaders = removeHeaders;
        FillLabels();
        ApplyStylings();
    }

    private void ApplyStylings()
    {
        if (removeHeaders)
        {
            tableLayoutPanelLabels.RowStyles[0].Height = 0;
            fileName.Visible = false;
            sendRole.Visible = false;
        }

        var senderColor = fileMetadataMessage.Sender.Id == localParticipant.Id
            ? ColorScheme.OutgoingMessageBackground
            : ColorScheme.IncomingMessageBackground;

        fileName.BackColor = senderColor;
        sendRole.BackColor = senderColor;
        tableLayoutPanel.BackColor = senderColor;
        sendTime.BackColor = senderColor;
        tableLayoutPanelLabels.BackColor = senderColor;

        fileName.Font = FontScheme.Monospace;
        sendRole.Font = FontScheme.MicroCaption;
        sendTime.Font = FontScheme.MicroCaption;
        fileName.Font = FontScheme.Default;
        fileSize.Font = FontScheme.Monospace;
        fileType.Font = FontScheme.Monospace;
    }

    private void FillLabels()
    {
        senderName.Text = fileMetadataMessage.Sender.Name;
        sendRole.Text = hostMessage ? "Хост" : string.Empty;
        sendTime.Text = fileMetadataMessage.Timestamp.ToShortTimeString();

        fileName.Text = fileMetadataMessage.FileName;
        fileSize.Text = fileMetadataMessage.FileSize.ToString();
        fileType.Text = fileMetadataMessage.FileName.Split('.')[1];
    }
}
