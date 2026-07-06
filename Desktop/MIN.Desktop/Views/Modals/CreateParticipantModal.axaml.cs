using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Modals;

/// <summary>
/// Всплывающее окно создания комнаты
/// </summary>
[ModalForViewModel(typeof(CreateParticipantViewModel))]
public partial class CreateParticipantModal : ModalViewBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CreateParticipantModal"/>
    /// </summary>
    public CreateParticipantModal()
    {
        InitializeComponent();
    }
}
