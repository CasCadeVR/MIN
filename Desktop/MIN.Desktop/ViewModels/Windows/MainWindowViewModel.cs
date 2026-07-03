using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models.ReferenceCommands;
using MIN.Desktop.Infrastructure.Extensions;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Pages;

namespace MIN.Desktop.ViewModels.Windows;

/// <summary>
/// Модель главного окна
/// </summary>
public partial class MainWindowViewModel : ViewModelBase, IMultiRoutingWindow
{
    /// <inheritdoc />
    [ObservableProperty]
    public partial object? LeftSideBarViewModel { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial object? ActiveViewModel { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial object? RightSideBarViewModel { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainWindowViewModel"/>
    /// </summary>
    public MainWindowViewModel(MainSideBarViewModel mainSideBarViewModel, DiscoveryViewModel discoveryViewModel)
    {
        this.RegisterMessageListener<ShowViewReferenceCommand, MainWindowViewModel>(static (message, vm)
            => vm.ShowAsync(message.ViewModel));
        this.RegisterMessageListener<ShowPreviousViewReferenceCommand, MainWindowViewModel>(static (message, vm)
            => vm.BackToAsync(message.RoutableViewModelType, message.LayoutType));

        //this.RegisterMessageListener<NotificationAddMessage, MainWindowViewModel>(static async (message, vm) =>
        //{
        //    Dispatcher.UIThread.Invoke(() =>
        //    {
        //        vm.Notifications.Add(message.Item);
        //    });
        //    await Task.Delay(7000);
        //    WeakReferenceMessenger.Default.Send(new NotificationCloseMessage(message.Item));
        //});
        //this.RegisterMessageListener<NotificationCloseMessage, MainWindowViewModel>(static async (message, vm) =>
        //{
        //    message.Item.IsDismissed = true;
        //    await Task.Delay(1000); // Wait for animations
        //    if (!IsDesignMode) // Prevent design preview crashes
        //    {
        //        Dispatcher.UIThread.Invoke(() =>
        //        {
        //            vm.Notifications.Remove(message.Item);
        //        });
        //    }
        //});

        _ = this.ShowAsync(mainSideBarViewModel);
        _ = this.ShowAsync(discoveryViewModel);
    }

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
