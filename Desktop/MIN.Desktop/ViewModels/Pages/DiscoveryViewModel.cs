using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель обнаружения комнат
/// </summary>
public partial class DiscoveryViewModel : RoutableViewModelBase
{
    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.Central;
}
