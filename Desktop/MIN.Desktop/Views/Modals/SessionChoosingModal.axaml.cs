using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Modals;

/// <summary>
/// Всплывающее окно подключения напрямую
/// </summary>
[ModalForViewModel(typeof(SessionChoosingViewModel))]
public partial class SessionChoosingModal : ModalViewBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionChoosingModal"/>
    /// </summary>
    public SessionChoosingModal()
    {
        InitializeComponent();
    }
}
