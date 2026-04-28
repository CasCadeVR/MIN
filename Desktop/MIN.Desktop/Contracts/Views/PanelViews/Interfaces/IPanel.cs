using MIN.Desktop.Contracts.Enums;

namespace MIN.Desktop.Contracts.Views.PanelViews.Interfaces;

/// <summary>
/// Представляет панель на экране
/// </summary>
public interface IPanel
{
    /// <summary>
    /// Тип панели
    /// </summary>
    PanelType PanelType { get; }

    /// <summary>
    /// Действия, которые необходимо выполнить при навигации на панель
    /// </summary>
    void OnNavigatedTo();
}
