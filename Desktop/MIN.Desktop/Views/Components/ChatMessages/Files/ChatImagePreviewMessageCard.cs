using MIN.Desktop.Contracts.Schemes;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения от пользователя
/// </summary>
public partial class ChatImagePreviewMessageCard : UserControl
{
    private readonly FileMetadataMessage fileMetadataMessage;
    private readonly bool isLocal;
    private readonly bool removeHeaders;
    private readonly bool hostMessage;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatImagePreviewMessageCard"/>
    /// </summary>
    public ChatImagePreviewMessageCard(FileMetadataMessage fileMetadataMessage,
        bool isLocal,
        bool hostMessage,
        bool removeHeaders)
    {
        InitializeComponent();

        this.fileMetadataMessage = fileMetadataMessage;
        this.hostMessage = hostMessage;
        this.isLocal = isLocal;
        this.removeHeaders = removeHeaders;

        FillLabels();
        ApplyStylings();
    }

    /// <inheritdoc />
    public void ApplyStylings()
    {
        if (removeHeaders)
        {
            tableLayoutPanelLabels.RowStyles[0].Height = 0;
            senderName.Visible = false;
            sendRole.Visible = false;
        }

        var senderColor = isLocal
            ? ColorScheme.OutgoingMessageBackground
            : ColorScheme.IncomingMessageBackground;

        senderName.BackColor = senderColor;
        sendRole.BackColor = senderColor;
        sendTime.BackColor = senderColor;
        tableLayoutPanelLabels.BackColor = senderColor;
        sendMessage.BackColor = senderColor;

        senderName.Font = FontScheme.Monospace;
        sendRole.Font = FontScheme.MicroCaption;
        sendTime.Font = FontScheme.MicroCaption;
        sendMessage.Font = FontScheme.Default;
    }

    private void FillLabels()
    {
        sendMessage.Text = fileMetadataMessage.FileName;
    }
}
