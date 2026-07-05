using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели чата
/// </summary>
public partial class ChatSideBarViewModel : RoutableViewModelBase
{
    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.RightSideBar;

    /// <summary>
    /// Вернуться назад
    /// </summary>
    [RelayCommand]
    public void CloseAsync() => ChangeViewToPrevious();
}
