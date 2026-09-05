using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace MIN.Desktop.Infrastructure.Behaviors;

/// <summary>
/// <see cref="Behavior.AssociatedObject"/> с перезаписывающим контекстным окном
/// </summary>
public class CustomContextMenuBehavior : Behavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject != null)
        {
            AssociatedObject.ContextRequested += AssociatedObject_ContextRequested;

            AssociatedObject.AddHandler(
                InputElement.PointerPressedEvent,
                AssociatedObject_PreviewPointerPressed,
                RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject != null)
        {
            AssociatedObject.ContextRequested -= AssociatedObject_ContextRequested;

            AssociatedObject.RemoveHandler(
                InputElement.PointerPressedEvent,
                AssociatedObject_PreviewPointerPressed);
        }
    }

    private void AssociatedObject_ContextRequested(object? sender, ContextRequestedEventArgs e)
    {
        e.Handled = true;
    }

    private void AssociatedObject_PreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject == null)
        {
            return;
        }

        var pointerProperties = e.GetCurrentPoint(AssociatedObject).Properties;

        if (pointerProperties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            e.Handled = true;

            var messageRowBorder = AssociatedObject.FindAncestorOfType<Border>(false, b => b.ContextFlyout != null);

            if (messageRowBorder?.ContextFlyout != null)
            {
                messageRowBorder.ContextFlyout.ShowAt(AssociatedObject);
            }
        }
    }
}
