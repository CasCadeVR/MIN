using Avalonia.Controls;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Windows;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views;

/// <summary>
/// Главное окно
/// </summary>
public partial class MainWindow : WindowEx<MainWindowViewModel>
{
    /// <summary>
    /// Текущий Layout
    /// </summary>
    public WindowLayout CurrentLayout { get; private set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainWindow"/>
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        CalculateResize(e.NewSize.Width);
    }

    private void CalculateResize(double width)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        var mode = width switch
        {
            > 1100 => WindowLayout.ThreeColumns,
            > 600 => WindowLayout.TwoColumns,
            _ => WindowLayout.Narrow
        };
        CurrentLayout = mode;
        vm.UpdateLayout(mode);
    }
}
