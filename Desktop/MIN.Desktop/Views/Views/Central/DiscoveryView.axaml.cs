using MIN.Desktop.ViewModels.Pages;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Central;

/// <summary>
/// Страница обнаружения комнат
/// </summary>
public partial class DiscoveryView : RoutableViewBase<DiscoveryViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryView"/>
    /// </summary>
    public DiscoveryView()
    {
        InitializeComponent();
    }
}
