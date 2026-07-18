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
    /// Инициализирует новый экземпляр <see cref="MainWindow"/>
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        var width = e.NewSize.Width;
        var mode = width switch
        {
            > 1100 => WindowLayout.ThreeColumns,
            > 600 => WindowLayout.TwoColumns,
            _ => WindowLayout.Narrow
        };
        vm.UpdateLayout(mode);
    }
}
