using MIN.Desktop.Contracts.Models.Enums;
using MIN.Desktop.Contracts.Views.PanelViews;

namespace MIN.Desktop.Contracts.Models;

/// <summary>
/// Модель навигации
/// </summary>
public class NavigationItem
{
    /// <summary>
    /// Название
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Изображение
    /// </summary>
    public Image? Icon { get; set; }

    /// <summary>
    /// Экземпляр страницы, который нужно показать
    /// </summary>
    public BasePanelView ViewInstance { get; set; } = null!;

    /// <summary>
    /// Тип панели
    /// </summary>
    public PanelType PanelType { get; set; }

    /// <summary>
    /// Родительская страница
    /// </summary>
    public NavigationItem? Parent { get; set; }
}
