using MIN.Desktop.ViewModels.Pages;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Pages;

/// <summary>
/// Страница боковой панели
/// </summary>
public partial class MainSideBarPage : RoutableViewBase<MainSideBarViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainSideBarPage"/>
    /// </summary>
    public MainSideBarPage()
    {
        InitializeComponent();
    }
}
