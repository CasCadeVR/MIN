using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Infrastructure.Extensions;

namespace MIN.Desktop.TemplatedControls;

/// <summary>
/// Кастомная полоска на окне
/// </summary>
public partial class CustomTitleBar : TemplatedControl
{
    /// <summary>
    /// Показать название приложения
    /// </summary>
    public readonly static DirectProperty<CustomTitleBar, bool> ShowTitleProperty =
       AvaloniaProperty.RegisterDirect<CustomTitleBar, bool>(
           nameof(showTitle),
           o => o.showTitle,
           (o, v) => o.showTitle = v, true);

    /// <summary>
    /// Может максимизироваться
    /// </summary>
    public readonly static DirectProperty<CustomTitleBar, bool> CanMaximizeProperty =
        AvaloniaProperty.RegisterDirect<CustomTitleBar, bool>(
            nameof(CanMaximize),
            o => o.CanMaximize,
            (o, v) => o.CanMaximize = v, true);

    /// <summary>
    /// Может минимизироваться
    /// </summary>
    public readonly static DirectProperty<CustomTitleBar, bool> CanMinimizeProperty =
        AvaloniaProperty.RegisterDirect<CustomTitleBar, bool>(
            nameof(CanMinimize),
            o => o.CanMinimize,
            (o, v) => o.CanMinimize = v, true);

    /// <summary>
    /// Может закрыться
    /// </summary>
    public readonly static DirectProperty<CustomTitleBar, bool> CanCloseProperty =
        AvaloniaProperty.RegisterDirect<CustomTitleBar, bool>(
            nameof(CanClose),
            o => o.CanClose,
            (o, v) => o.CanClose = v, true);

    private bool showTitle = true;
    private bool canMaximize = true;
    private bool canMinimize = true;
    private bool canClose = true;

    /// <summary>
    /// Показать свойство
    /// </summary>
    public bool ShowTitle
    {
        get => showTitle;
        set => SetAndRaise(ShowTitleProperty, ref showTitle, value);
    }

    /// <summary>
    /// Может максимизироваться
    /// </summary>
    public bool CanMaximize
    {
        get => canMaximize;
        set => SetAndRaise(CanMaximizeProperty, ref canMaximize, value);
    }

    /// <summary>
    /// Может минимизироваться
    /// </summary>
    public bool CanMinimize
    {
        get => canMinimize;
        set => SetAndRaise(CanMinimizeProperty, ref canMinimize, value);
    }

    /// <summary>
    /// Может закрыться
    /// </summary>
    public bool CanClose
    {
        get => canClose;
        set => SetAndRaise(CanCloseProperty, ref canClose, value);
    }

    /// <summary>
    /// Минимизироваться
    /// </summary>
    [RelayCommand]
    public void Minimize()
    {
        if (!CanMinimize)
        {
            return;
        }
        if (this.GetWindow() is not { } window)
        {
            return;
        }
        window.WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Максимизироваться
    /// </summary>
    [RelayCommand]
    public void Maximize()
    {
        if (!CanMaximize)
        {
            return;
        }
        if (this.GetWindow() is not { } window)
        {
            return;
        }
        window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
    }

    ///// <summary>
    ///// Событие при загрузке
    ///// </summary>
    //protected override void OnLoaded(RoutedEventArgs e)
    //{
    //    PointerPressed += OnPointerPressed;
    //    DoubleTapped += OnDoubleTapped;
    //    base.OnLoaded(e);
    //}

    /// <summary>
    /// Событие при закрытии
    /// </summary>
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        PointerPressed -= OnPointerPressed;
        DoubleTapped -= OnDoubleTapped;
        base.OnUnloaded(e);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Source is Visual element && element.GetWindow() is { } window)
        {
            window.BeginMoveDrag(e);
        }
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e) => Maximize();

    [RelayCommand]
    private void Close()
    {
        if (this.GetWindow() is not { } window)
        {
            return;
        }
        window.Close();
    }
}
