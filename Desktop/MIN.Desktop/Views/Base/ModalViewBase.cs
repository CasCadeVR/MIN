using Avalonia.Controls;
using MIN.Desktop.Infrastructure.Extensions;

namespace MIN.Desktop.Views.Base;

/// <summary>
/// Базовая view для модальных окон
/// </summary>
public class ModalViewBase : Window
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        this.ApplyPlatformWindowStyle();
    }
}
