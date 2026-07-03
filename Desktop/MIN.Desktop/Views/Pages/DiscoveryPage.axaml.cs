using MIN.Desktop.ViewModels.Pages;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Pages;

/// <summary>
/// Страница обнаружения комнат
/// </summary>
public partial class DiscoveryPage : RoutableViewBase<DiscoveryViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryPage"/>
    /// </summary>
    public DiscoveryPage()
    {
        InitializeComponent();
    }
}
