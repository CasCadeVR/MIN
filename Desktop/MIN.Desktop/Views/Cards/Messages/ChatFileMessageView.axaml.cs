using MIN.Desktop.ViewModels.Cards.Messages.Files;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Cards.Messages;

/// <summary>
/// Файловое сообщение
/// </summary>
public partial class ChatFileMessageView : CardViewBase<ChatFileMessageViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileMessageView"/>
    /// </summary>
    public ChatFileMessageView()
    {
        InitializeComponent();
    }
}
