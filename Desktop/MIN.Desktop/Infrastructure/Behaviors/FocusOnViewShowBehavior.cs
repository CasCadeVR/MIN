using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;

namespace MIN.Desktop.Infrastructure.Behaviors;

/// <summary>
/// Фокусируется на <see cref="Behavior.AssociatedObject" /> когда его родитель показывается
/// </summary>
public class FocusOnViewShowBehavior : Behavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        WeakReferenceMessenger.Default.Register<ShowViewReferenceCommand>(this, static (obj, _) =>
        {
            // Need to queue this work on UI thread as calls via message originate from a different thread.
            Dispatcher.UIThread.Invoke(() => (obj as FocusOnViewShowBehavior)?.Focus());
        });
        base.OnAttached();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        WeakReferenceMessenger.Default.Unregister<ShowViewReferenceCommand>(this);
        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree() => Focus();

    private void Focus()
    {
        if (AssociatedObject is not { IsEffectivelyVisible: true })
        {
            return;
        }

        AssociatedObject.Focus();
        if (AssociatedObject is TextBox textBox)
        {
            textBox.SelectAll();
        }
    }
}
