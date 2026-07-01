using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Windows;

/// <summary>
/// Модель главного окна
/// </summary>
public partial class MainWindowModel : ViewModelBase
{
    [RelayCommand]
    private void Minimize()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Windows[0].WindowState = WindowState.Minimized;
        }
    }

    [RelayCommand]
    private void Maximize()
    {
        var window = GetWindow();
        if (window is null)
        {
            return;
        }

        window.WindowState = window.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    [RelayCommand]
    private void Close()
    {
        GetWindow()?.Close();
    }

    private static Window? GetWindow() =>
        Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime
            ? lifetime.Windows[0]
            : null;
}
