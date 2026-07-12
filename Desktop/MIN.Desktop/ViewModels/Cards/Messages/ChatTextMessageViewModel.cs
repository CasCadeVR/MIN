using MIN.Chat.Messaging;

namespace MIN.Desktop.ViewModels.Cards.Messages;

/// <summary>
/// Текстовое сообщение участника
/// </summary>
public partial class ChatTextMessageViewModel : BaseChatMessageViewModel
{
    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    public ChatTextMessage ChatMessage { get; init; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatTextMessageViewModel"/>
    /// </summary>
    public ChatTextMessageViewModel(ChatTextMessage chatMessage,
        bool isLocal,
        bool isHostMessage,
        bool removeHeaders)
        : base(chatMessage.Sender.Name,
            chatMessage.Timestamp,
            isLocal,
            isHostMessage,
            removeHeaders)
    {
        ChatMessage = chatMessage;
    }

    ///// <summary>
    ///// Подстроивает размеры сообщений под текст внутри и возвращает полученную высоту
    ///// </summary>
    ///// <returns>
    ///// Вычисленную высоту, исходя из содержимого
    ///// </returns>
    //int IResizableComponent.ResizeOutOfPrefferedSize()
    //{
    //    var wantedWidth = Math.Min(Convert.ToInt32(Parent!.Width * 0.85),
    //        Convert.ToInt32(TableLayoutPanel.ColumnStyles[1].Width)
    //        + Math.Max(sendMessage.PreferredSize.Width, removeHeaders ? 0 : senderName.PreferredSize.Width)
    //        + sendMessage.Margin.Horizontal * 2);

    //    if (Width == wantedWidth)
    //    {
    //        return Height;
    //    }

    //    Width = wantedWidth;

    //    var lineCount = CalculateLineCount();

    //    var gottenHeight = Convert.ToInt32(TableLayoutPanel.RowStyles[0].Height)
    //        + (lineCount * (sendMessage.Font.Height - 1))
    //        + sendMessage.Margin.Vertical * 2;

    //    Height = gottenHeight;
    //    return gottenHeight;
    //}

    //private int CalculateLineCount()
    //{
    //    var lastCharLine = sendMessage.GetLineFromCharIndex(sendMessage.Text.Length - 1);
    //    var resultLines = Math.Max(1, lastCharLine + 1);
    //    return resultLines;
    //}
}
