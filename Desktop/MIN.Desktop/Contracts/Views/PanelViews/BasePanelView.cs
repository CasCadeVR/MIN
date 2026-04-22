using MIN.Desktop.Contracts.Models;

namespace MIN.Desktop.Contracts.Views.PanelViews;

/// <summary>
/// Базовая вкладка
/// </summary>
public partial class BasePanelView : UserControl
{
    /// <summary>
    /// Текущая модель навигации
    /// </summary>
    public NavigationItem CurrentNavigationItem = null!;

    /// <summary>
    /// Запрос навигации
    /// </summary>
    public event Action<NavigationItem> RequestNavigate = null!;

    /// <summary>
    /// Инициализирует новый экземляр <see cref="BasePanelView"/>
    /// </summary>
    public BasePanelView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Инициализирует новый экземляр <see cref="BasePanelView"/>
    /// </summary>
    public BasePanelView(NavigationItem item)
    {
        InitializeComponent();
        CurrentNavigationItem = item;
    }

    private void OnBackButtonClick(object? sender, EventArgs e)
    {
        NavigateToParent();
    }

    /// <summary>
    /// Инициализация панели всякий раз, когда на неё приходят
    /// </summary>
    public virtual void OnNavigation(NavigationItem item)
    {
        CurrentNavigationItem = item;
    }

    /// <summary>
    /// Перейти к родителю
    /// </summary>
    protected void NavigateToParent()
    {
        if (CurrentNavigationItem?.Parent != null)
        {
            RequestNavigate?.Invoke(CurrentNavigationItem.Parent);
        }
    }
}
