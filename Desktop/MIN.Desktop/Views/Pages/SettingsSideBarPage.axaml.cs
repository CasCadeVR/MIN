using MIN.Desktop.ViewModels.Pages;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Pages;

/// <summary>
/// Страница боковой панели
/// </summary>
public partial class SettingsSideBarPage : RoutableViewBase<SettingsSideBarViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SettingsSideBarPage"/>
    /// </summary>
    public SettingsSideBarPage()
    {
        InitializeComponent();
    }
}
