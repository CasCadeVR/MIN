namespace MIN.Desktop.ViewModels.Cards.Messages;

/// <summary>
/// Системное сообщение
/// </summary>
public partial class SystemChatMessageViewModel : BaseChatMessageViewModel
{
    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SystemChatMessageViewModel"/>
    /// </summary>
    public SystemChatMessageViewModel() : base() { }
}
