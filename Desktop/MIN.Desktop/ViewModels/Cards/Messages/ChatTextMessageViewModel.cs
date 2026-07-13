using Avalonia;
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
        Thickness timePadding,
        bool isLocal,
        bool isHostMessage,
        bool removeHeaders)
        : base(chatMessage.Sender.Name,
            chatMessage.Timestamp,
            timePadding,
            isLocal,
            isHostMessage,
            removeHeaders)
    {
        ChatMessage = chatMessage;
    }
}
