using MIN.Chat.Messaging;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения от пользователя
/// </summary>
public partial class ChatTextMessageCard : UserControl, IStyled
{
    private readonly ChatTextMessage chatMessage;
    private readonly bool hostMessage;
    private readonly bool isLocal;
    private readonly bool removeHeaders;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatTextMessageCard"/>
    /// </summary>
    public ChatTextMessageCard(ChatTextMessage chatMessage,
        bool isLocal,
        bool hostMessage,
        bool removeHeaders)
    {
        InitializeComponent();

        this.chatMessage = chatMessage;
        this.hostMessage = hostMessage;
        this.isLocal = isLocal;
        this.removeHeaders = removeHeaders;

        ApplyStylings();
        FillLabels();
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

    /// <summary>
    /// Подстроивает размеры сообщений под текст внутри и возвращает полученную высоту
    /// </summary>
    /// <returns>
    /// Вычисленную высоту, исходя из содержимого
    /// </returns>
    public int ResizeOutOfPrefferedSize()
    {
        var wantedWidth = Math.Min(Convert.ToInt32(Parent!.Width * 0.85),
            Convert.ToInt32(tableLayoutPanelLabels.ColumnStyles[1].Width)
            + Math.Max(sendMessage.PreferredSize.Width, removeHeaders ? 0 : sendMessage.PreferredSize.Width)
            + sendMessage.Margin.Horizontal * 2);

        if (Width == wantedWidth)
        {
            return Height;
        }

        Width = wantedWidth;

        var lineCount = CalculateLineCount();

        var gottenHeight = Convert.ToInt32(tableLayoutPanelLabels.RowStyles[0].Height)
            + (lineCount * (sendMessage.Font.Height - 1))
            + sendMessage.Margin.Vertical * 2;

        Height = gottenHeight;
        return gottenHeight;
    }

    private int CalculateLineCount()
    {
        var lastCharLine = sendMessage.GetLineFromCharIndex(sendMessage.Text.Length - 1);
        var resultLines = Math.Max(1, lastCharLine + 1);
        return resultLines;
    }

    private void FillLabels()
    {
        senderName.Text = chatMessage.Sender.Name;
        sendRole.Text = hostMessage ? "Хост" : string.Empty;
        sendTime.Text = chatMessage.Timestamp.ToShortTimeString();
        sendMessage.Text = chatMessage.Content;
    }
}
