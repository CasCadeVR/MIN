using System;
using Avalonia.Collections;
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
    private readonly ILoggerProvider loggerProvider;
    private int currentPage;

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
        loggerProvider.OnLogReceived += OnLogReceived;
    }

    private void OnLogReceived(object? sender, LogItem item)
    {
        AddLogMessage(item);
    }

    private void AddLogMessage(LogItem item)
    {
        LogItems.Insert(0, new LogItemViewModel(item));
    }

    [RelayCommand]
    private void LoadLogs()
    {
        var history = loggerProvider.GetRecentLogHistory(currentPage, 100);

        foreach (var item in history)
        {
            LogItems.Add(new LogItemViewModel(item));
        }

        //logListBox.TopIndex = 0;
        //logListBox.Update();
    }

    [RelayCommand]
    private void Unsubscribe()
    {
        loggerProvider.OnLogReceived -= OnLogReceived;
    }

    private void loadMoreButton_Click(object sender, EventArgs e)
    {
        currentPage++;
        LoadLogs();
        //var visibleItems = logListBox.ClientSize.Height / logListBox.ItemHeight;
        //logListBox.TopIndex = Math.Max(LogItems.Count - visibleItems + 1, 0);
    }

    private void scrollUpButton_Click(object sender, EventArgs e)
    {
        //logListBox.TopIndex = 0;
    }
}
