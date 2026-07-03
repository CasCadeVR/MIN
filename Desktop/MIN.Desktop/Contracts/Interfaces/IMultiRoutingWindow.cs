namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Окно, предаставляющее маршрутизацию по страницам
/// имея возможность навигироваться по 3 страницам за раз
/// </summary>
public interface IMultiRoutingWindow
{
    /// <summary>
    /// Левая страница
    /// </summary>
    object? LeftSideBarViewModel { get; set; }

    /// <summary>
    /// Центральная страница
    /// </summary>
    object? ActiveViewModel { get; set; }

    /// <summary>
    /// Правая страница
    /// </summary>
    object? RightSideBarViewModel { get; set; }
}
