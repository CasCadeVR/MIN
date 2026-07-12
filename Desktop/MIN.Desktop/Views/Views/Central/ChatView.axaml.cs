using MIN.Desktop.ViewModels.Pages.ChatViewModels;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Central;

/// <summary>
/// Страница чата
/// </summary>
public partial class ChatView : RoutableViewBase<ChatViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatView"/>
    /// </summary>
    public ChatView()
    {
        InitializeComponent();
    }
}
