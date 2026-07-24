using MIN.Desktop.ViewModels.Pages;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Right;

/// <summary>
/// Страница боковой панели чата
/// </summary>
public partial class ChatSideBarView : RoutableViewBase<ChatSideBarViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatSideBarView"/>
    /// </summary>
    public ChatSideBarView()
    {
        InitializeComponent();
    }
}
