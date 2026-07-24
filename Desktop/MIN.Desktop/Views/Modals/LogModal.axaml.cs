using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Modals;

/// <summary>
/// Всплывающее окно подключения напрямую
/// </summary>
[ModalForViewModel(typeof(LogViewModel))]
public partial class LogModal : ModalViewBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LogModal"/>
    /// </summary>
    public LogModal()
    {
        InitializeComponent();
    }
}
