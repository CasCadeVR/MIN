using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Xaml.Interactivity;

namespace MIN.Desktop.Infrastructure.Behaviors.ScrollViewers;

/// <summary>
/// Прокрутка <see cref="ScrollViewer"/> по горизонтали и вертикали
/// </summary>
public class PrimaryScrollWheelBehavior : StyledElementBehavior<ScrollViewer>
{
    /// <summary>
    /// Направления scroll
    /// </summary>
    public readonly static AttachedProperty<Orientation> PrimaryScrollWheelDirectionProperty
        = AvaloniaProperty.RegisterAttached<PrimaryScrollWheelBehavior, ScrollViewer, Orientation>("PrimaryScrollWheelDirection", Orientation.Vertical);

    /// <summary>
    /// Получить направление прокрутки
    /// </summary>
    public static Orientation GetPrimaryScrollWheelDirection(AvaloniaObject obj) => obj.GetValue(PrimaryScrollWheelDirectionProperty);

    /// <summary>
    /// Изменяет ввод с помощью колёсика мыши для перемещения средства просмотра содержимого влево и вправо, если установлено значение <see cref="Orientation.Horizontal" />.    
    /// </summary>
    public static void SetPrimaryScrollWheelDirection(AvaloniaObject obj, Orientation orientation)
    {
        obj.SetValue(PrimaryScrollWheelDirectionProperty, orientation);
        if (obj is not ScrollViewer scrollViewer)
        {
            return;
        }

        switch (orientation)
        {
            case Orientation.Horizontal:
                scrollViewer.PointerWheelChanged += RotatedOrientationWheelHandler;
                break;
            case Orientation.Vertical:
                scrollViewer.PointerWheelChanged -= RotatedOrientationWheelHandler;
                break;
        }

        static void RotatedOrientationWheelHandler(object? sender, PointerWheelEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
            {
                return;
            }
            if (GetPrimaryScrollWheelDirection(scrollViewer) == Orientation.Vertical)
            {
                return;
            }

            if (e.Delta.Y < 0)
            {
                for (var i = 0; i <= -e.Delta.Y; i++)
                {
                    scrollViewer.LineRight();
                }
            }
            else
            {
                for (var i = 0; i <= e.Delta.Y; i++)
                {
                    scrollViewer.LineLeft();
                }
            }
            e.Handled = true;
        }
    }
}
