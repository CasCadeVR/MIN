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
}
