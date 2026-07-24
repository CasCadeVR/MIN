using MIN.Desktop.ViewModels.Cards.Messages;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Cards.Messages;

/// <summary>
/// Текстовое сообщение
/// </summary>
public partial class ChatTextMessageView : CardViewBase<ChatTextMessageViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatTextMessageView"/>
    /// </summary>
    public ChatTextMessageView()
    {
        InitializeComponent();
    }
}
