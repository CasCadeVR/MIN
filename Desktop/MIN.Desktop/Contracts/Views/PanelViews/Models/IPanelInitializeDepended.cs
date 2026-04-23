namespace MIN.Desktop.Contracts.Views.PanelViews.Models;

/// <summary>
/// Инициализатор панели
/// </summary>
/// <typeparam name="TParams">Параметры, нужные для панели</typeparam>
public interface IPanelInitializeDepended<TParams>
{
    /// <summary>
    /// Инициализировать панель
    /// </summary>
    /// <param name="parameters">Параметры, нужные для панели</param>
    void Initialize(TParams parameters);
}
