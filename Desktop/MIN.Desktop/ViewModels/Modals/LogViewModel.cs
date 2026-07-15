using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Cards;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна логов
/// </summary>
public partial class LogViewModel : ModalViewModelBase
{
    private const int LogPageSize = 100;

    private readonly ILoggerProvider loggerProvider;
    private readonly IClipboard? clipboard;
    private int currentPage;

    /// <summary>
    /// Флаг переключения автоскролла вверх
    /// </summary>
    [ObservableProperty]
    public partial bool AutoScrollTop { get; set; }

    /// <summary>
    /// Флаг переключения автоскролла вниз
    /// </summary>
    [ObservableProperty]
    public partial bool AutoScrollBottom { get; set; }

    /// <summary>
    /// Список логов
    /// </summary>
    [ObservableProperty]
    public partial AvaloniaList<LogItemViewModel> LogItems { get; set; } = [];

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LogViewModel"/>
    /// </summary>
    public LogViewModel(ILoggerProvider loggerProvider)
    {
        this.loggerProvider = loggerProvider;

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            clipboard = lifetime.Windows[0].Clipboard;
        }

        if (!Design.IsDesignMode)
        {
            loggerProvider.OnLogReceived += OnLogReceived;
        }
    }

    private void OnLogReceived(object? sender, LogItem item)
    {
        AddLogMessage(item);
    }

    private void AddLogMessage(LogItem item)
    {
        LogItems.Insert(0, new LogItemViewModel(item, clipboard));
    }

    [RelayCommand]
    private async Task LoadLogs()
    {
        var history = loggerProvider.GetRecentLogHistory(currentPage, LogPageSize);

        foreach (var item in history)
        {
            LogItems.Add(new LogItemViewModel(item, clipboard));
        }

        await ScrollToTop();
    }

    [RelayCommand]
    private void Unsubscribe()
    {
        loggerProvider.OnLogReceived -= OnLogReceived;
    }

    [RelayCommand]
    private async Task LoadMore()
    {
        currentPage++;
        await LoadLogs();
        await ScrollToBottom();
    }

    [RelayCommand]
    private async Task ScrollUp()
    {
        await ScrollToTop();
    }

    private async Task ScrollToTop()
    {
        AutoScrollTop = true;
        await Task.Yield();
        AutoScrollTop = false;
    }

    private async Task ScrollToBottom()
    {
        AutoScrollBottom = true;
        await Task.Yield();
        AutoScrollBottom = false;
    }
}
