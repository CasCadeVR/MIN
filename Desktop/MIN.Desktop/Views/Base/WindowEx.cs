using Avalonia.Controls;
using MIN.Desktop.Infrastructure.Extensions;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.Views.Base;

/// <summary>
/// Базовая view для немодальных окон
/// </summary>
public class WindowEx<TViewModel> : Window
    where TViewModel : ViewModelBase
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        this.ApplyPlatformWindowStyle();
    }
}
