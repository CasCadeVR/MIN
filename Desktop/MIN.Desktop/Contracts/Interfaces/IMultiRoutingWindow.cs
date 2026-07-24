using System.Collections.Generic;
using System.Threading;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Окно, предаставляющее маршрутизацию по страницам
/// имея возможность навигироваться по 3 страницам за раз
/// </summary>
public interface IMultiRoutingWindow
{
    /// <summary>
    /// Словарь сохранённого стека навигации
    /// </summary>
    Dictionary<ViewLayoutType, List<IRoutableViewModel>> NavigationStack { get; }

    /// <summary>
    /// Словарь загрузок страниц
    /// </summary>
    Dictionary<ViewLayoutType, CancellationTokenSource?> ViewChangeBusyCtsByLayout { get; }

    /// <summary>
    /// Layout на странице
    /// </summary>
    WindowLayout LayoutMode { get; }

    /// <summary>
    /// Левая страница
    /// </summary>
    object? LeftSideBarViewModel { get; set; }

    /// <summary>
    /// Центральная страница
    /// </summary>
    object? CentralViewModel { get; set; }

    /// <summary>
    /// Правая страница
    /// </summary>
    object? RightSideBarViewModel { get; set; }

    /// <summary>
    /// Получить переменную view model из типа layout
    /// </summary>
    /// <returns></returns>
    object? GetViewModelOutOfLayoutType(ViewLayoutType type);
}
