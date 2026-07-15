using System;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.ViewModels.Cards;

/// <summary>
/// View модель строчки лога
/// </summary>
public partial class LogItemViewModel : CardViewModelBase, IDisposable
{
    private readonly IClipboard? clipboard;

    /// <summary>
    /// Сообщение
    /// </summary>
    public LogItem LogItem { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LogItemViewModel"/>
    /// </summary>
    public LogItemViewModel(LogItem logItem, IClipboard? clipboard)
    {
        LogItem = logItem;
        this.clipboard = clipboard;
    }

    [RelayCommand]
    private async Task CopyToClipBoard(string message)
    {
        if (clipboard != null)
        {
            await clipboard.SetTextAsync(message);
            InAppNotifier.Info("Скопировано в буфер обмена");
        }
    }
}
