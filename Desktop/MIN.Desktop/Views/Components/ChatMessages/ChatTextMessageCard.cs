using MIN.Chat.Messaging;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Views.Components.ChatMessages;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения от пользователя
/// </summary>
public partial class ChatTextMessageCard : BaseChatMessageCard
{
    private readonly ChatTextMessage chatMessage = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatTextMessageCard"/>
    /// </summary>
    public ChatTextMessageCard() : base()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatTextMessageCard"/>
    /// </summary>
    public ChatTextMessageCard(ChatTextMessage chatMessage,
        bool isLocal,
        bool isHostMessage,
        bool removeHeaders)
        : base(chatMessage.Sender.Name,
            chatMessage.Timestamp,
            isLocal,
            isHostMessage,
            removeHeaders)
    {
        InitializeComponent();

        this.chatMessage = chatMessage;

        ApplyStylings();
        FillLabels();
    }

    /// <inheritdoc />
    public override void ApplyStylings()
    {
        base.ApplyStylings();
        sendMessage.BackColor = SenderColor;
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
            Convert.ToInt32(TableLayoutPanel.ColumnStyles[1].Width)
            + Math.Max(sendMessage.PreferredSize.Width, removeHeaders ? 0 : sendMessage.PreferredSize.Width)
            + sendMessage.Margin.Horizontal * 2);

        if (Width == wantedWidth)
        {
            return Height;
        }

        Width = wantedWidth;

        var lineCount = CalculateLineCount();

        var gottenHeight = Convert.ToInt32(TableLayoutPanel.RowStyles[0].Height)
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
        sendMessage.Text = chatMessage.Content;
    }
}
