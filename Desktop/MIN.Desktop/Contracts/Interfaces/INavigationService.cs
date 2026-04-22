using MIN.Desktop.Contracts.Models;

namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Сервис навигации
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Перейти по <see cref="NavigationItem"/>
    /// </summary>
    void NavigateTo(NavigationItem item);
}
