using Avalonia;
using Avalonia.Controls;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="Visual"/>
/// </summary>
public static class VisualExtensions
{
    /// <summary>
    /// Получить контекст окна
    /// </summary>
    public static Window? GetWindow(this Visual visual) => TopLevel.GetTopLevel(visual) as Window;
}
