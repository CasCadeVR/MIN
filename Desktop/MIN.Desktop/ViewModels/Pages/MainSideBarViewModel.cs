using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели
/// </summary>
public partial class MainSideBarViewModel : RoutableViewModelBase
{
    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.LeftSideBar;
}
