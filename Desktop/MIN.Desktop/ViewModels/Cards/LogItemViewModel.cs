using System;
using MIN.Desktop.ViewModels.Base;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.ViewModels.Cards;

/// <summary>
/// View модель строчки лога
/// </summary>
public partial class LogItemViewModel : CardViewModelBase, IDisposable
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public LogItem LogItem { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LogItemViewModel"/>
    /// </summary>
    public LogItemViewModel(LogItem logItem)
    {
        LogItem = logItem;
    }
}
