using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace MIN.Desktop.Contracts.Behaviors;

/// <summary>
/// Прокрутка <see cref="ScrollViewer"/> вниз
/// </summary>
public class AutoScrollToBottomBehavior : StyledElementBehavior<ScrollViewer>
{
    /// <summary>
    /// Авто скролл
    /// </summary>
    public readonly static AttachedProperty<bool> AutoScrollProperty =
        AvaloniaProperty.RegisterAttached<AutoScrollToBottomBehavior, ScrollViewer, bool>("AutoScroll");

    static AutoScrollToBottomBehavior()
    {
        AutoScrollProperty.Changed.AddClassHandler<ScrollViewer>(OnAutoScrollChanged);
    }

    /// <summary>
    /// Установить авто скролл
    /// </summary>
    public static void SetAutoScroll(ScrollViewer element, bool value) =>
        element.SetValue(AutoScrollProperty, value);

    /// <summary>
    /// Получить авто скролл
    /// </summary>
    public static bool GetAutoScroll(ScrollViewer element) =>
        element.GetValue(AutoScrollProperty);

    private static void OnAutoScrollChanged(ScrollViewer sv, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            sv.Tag = true; // user hasn't scrolled up
            sv.ScrollChanged += OnScrollChanged;
        }
        else
        {
            sv.ScrollChanged -= OnScrollChanged;
        }
    }

    private static void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer sv)
        {
            return;
        }

        // Detect user scroll up
        if (e.OffsetDelta.Y < 0)
        {
            sv.Tag = false; // user scrolled up, stop auto-scroll
        }

        // Auto-scroll if enabled and at bottom
        if ((bool?)sv.Tag == true)
        {
            sv.ScrollToEnd();
        }

        // If user scrolls back to bottom, re-enable auto-scroll
        if (sv.Offset.Y + sv.Viewport.Height >= sv.Extent.Height - 1)
        {
            sv.Tag = true;
        }
    }
}
