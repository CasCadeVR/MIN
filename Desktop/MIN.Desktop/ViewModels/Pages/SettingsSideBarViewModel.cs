using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели настроек
/// </summary>
public partial class SettingsSideBarViewModel : RoutableViewModelBase
{
    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.LeftSideBar;

    /// <summary>
    /// Включена ли светлая тема
    /// </summary>
    [ObservableProperty]
    public partial bool IsLightModeEnabled { get; set; }

    partial void OnIsLightModeEnabledChanged(bool value)
    {
        Dispatcher.UIThread.Invoke(() => Application.Current!.RequestedThemeVariant = value ? ThemeVariant.Light : ThemeVariant.Dark);
    }
}
