using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Modals;

/// <summary>
/// Всплывающее окно диалога
/// </summary>
[ModalForViewModel(typeof(DialogBoxViewModel))]
public partial class DialogBoxView : ModalViewBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DialogBoxView"/>
    /// </summary>
    public DialogBoxView()
    {
        InitializeComponent();
    }
}
