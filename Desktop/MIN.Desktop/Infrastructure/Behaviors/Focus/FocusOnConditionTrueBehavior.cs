using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace MIN.Desktop.Infrastructure.Behaviors.Focus;

/// <summary>
/// Фокусируется на <see cref="Behavior.AssociatedObject" /> когда присоединённый binding становиться true
/// </summary>
public class FocusOnConditionTrueBehavior : Behavior<Control>
{
    /// <summary>
    /// Авто скролл вверх
    /// </summary>
    public readonly static AttachedProperty<bool> ConditionProperty =
        AvaloniaProperty.RegisterAttached<FocusOnConditionTrueBehavior, Control, bool>("Condition");

    static FocusOnConditionTrueBehavior()
    {
        ConditionProperty.Changed.AddClassHandler<Control>(OnConditionChanged);
    }

    /// <summary>
    /// Установить авто скролл вверх
    /// </summary>
    public static void SetCondition(Control element, bool value) =>
        element.SetValue(ConditionProperty, value);

    /// <summary>
    /// Получить авто скролл вверх
    /// </summary>
    public static bool GetCondition(Control element) =>
        element.GetValue(ConditionProperty);

    private static void OnConditionChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            Focus(control);
        }
    }

    private static void Focus(Control control)
    {
        if (control is not { IsEffectivelyVisible: true })
        {
            return;
        }

        control.Focus();
        if (control is TextBox textBox)
        {
            textBox.CaretIndex = textBox.Text?.Length ?? 0;
        }
    }
}
