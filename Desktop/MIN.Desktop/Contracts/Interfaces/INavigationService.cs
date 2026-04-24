using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Contracts.Views.PanelViews;

namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Сервис навигации
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Родительская форма
    /// </summary>
    BaseForm Parent { get; set; }

    /// <summary>
    /// Окно разделения
    /// </summary>
    SplitContainer SplitContainer { get; set; }

    /// <summary>
    /// Главная панель
    /// </summary>
    Panel MainPanel { get; set; }

    /// <summary>
    /// Боковая панель
    /// </summary>
    Panel SidePanel { get; set; }

    /// <summary>
    /// Перейти на панель
    /// </summary>
    /// <typeparam name="TPanel">Панель</typeparam>
    void NavigateTo<TPanel>() where TPanel : BasePanelView;

    /// <summary>
    /// Перейти на панель, предварительно инициализировав её параметры
    /// </summary>
    /// <typeparam name="TPanel">Панель</typeparam>
    /// <typeparam name="TParams">Параметры</typeparam>
    /// <param name="param">Параметры</param>
    void NavigateTo<TPanel, TParams>(TParams param) where TPanel : BasePanelView;
}
