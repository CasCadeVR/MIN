using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Contracts.Models.ReferenceCommands;
using MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;
using MIN.Desktop.Infrastructure.Extensions;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Pages;

namespace MIN.Desktop.ViewModels.Windows;

/// <summary>
/// Модель главного окна
/// </summary>
public partial class MainWindowViewModel : ViewModelBase, IMultiRoutingWindow
{
    [ObservableProperty]
    public partial WindowLayout LayoutMode { get; set; } = WindowLayout.ThreeColumns;

    /// <inheritdoc />
    [ObservableProperty]
    public partial object? LeftSideBarViewModel { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial object? CentralViewModel { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial object? RightSideBarViewModel { get; set; }

    /// <summary>
    /// Текущие уведомления внутри приложения
    /// </summary>
    public AvaloniaList<NotificationItem> Notifications { get; init; } = [];

    partial void OnLayoutModeChanged(WindowLayout value)
        => WeakReferenceMessenger.Default.Send(new LayoutModeChangedReferenceCommand(value));

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainWindowViewModel"/>
    /// </summary>
    public MainWindowViewModel(MainSideBarViewModel mainSideBarViewModel, DiscoveryViewModel discoveryViewModel)
    {
        this.RegisterMessageListener<ShowViewReferenceCommand, MainWindowViewModel>(static (message, vm)
            => vm.ShowAsync(message.ViewModel));
        this.RegisterMessageListener<CloseViewReferenceCommand, MainWindowViewModel>(static (message, vm)
            => vm.CloseAsync(message.LayoutType));
        this.RegisterMessageListener<ShowPreviousViewReferenceCommand, MainWindowViewModel>(async (message, vm)
            =>
        {
            if (message.RoutableViewModelType != null)
            {
                await vm.BackToAsync(message.RoutableViewModelType, message.LayoutType);
            }
            else
            {
                await vm.BackAsync(message.LayoutType);
            }
        });

        this.RegisterMessageListener<NotificationAddReferenceCommand, MainWindowViewModel>(static async (message, vm) =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                vm.Notifications.Add(message.NotificationItem);
            });
            await Task.Delay(7000);
            WeakReferenceMessenger.Default.Send(new NotificationCloseReferenceCommand(message.NotificationItem));
        });
        this.RegisterMessageListener<NotificationCloseReferenceCommand, MainWindowViewModel>(static async (message, vm) =>
        {
            message.NotificationItem.IsDismissed = true;

            await Task.Delay(1000); // Wait for animations

            if (!Design.IsDesignMode) // Prevent design preview crashes
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    vm.Notifications.Remove(message.NotificationItem);
                });
            }
        });

        this.RegisterMessageListener<ShowNavigationReferenceCommand, MainWindowViewModel>(async (message, vm) =>
        {
            // Это костыль, потому что discovery не может сослаться на mainSideBarViewModel
            await this.ShowAsync(mainSideBarViewModel);
        });

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

        // Assume that in maximize it would fit 3 columns
        UpdateLayout(WindowLayout.ThreeColumns);
    }

    [RelayCommand]
    private void Close()
    {
        GetWindow()?.Close();
    }

    /// <summary>
    /// Обновить разметку страницы из под размеров окнна
    /// </summary>
    public void UpdateLayout(WindowLayout newMode)
    {
        if (LayoutMode == newMode)
        {
            return;
        }

        LayoutMode = newMode;
        this.ArrangeLayout();
    }

    /// <summary>
    /// Получить окно приложения
    /// </summary>
    public static Window? GetWindow() =>
        Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime
            ? lifetime.Windows[0]
            : null;
}
