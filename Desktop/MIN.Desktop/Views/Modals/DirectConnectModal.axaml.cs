using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Modals;

/// <summary>
/// Всплывающее окно подключения напрямую
/// </summary>
[ModalForViewModel(typeof(DirectConnectViewModel))]
public partial class DirectConnectModal : ModalViewBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DirectConnectModal"/>
    /// </summary>
    public DirectConnectModal()
    {
        InitializeComponent();
    }
}
