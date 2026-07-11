using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Modals;

/// <summary>
/// Всплывающее окно кика участника
/// </summary>
[ModalForViewModel(typeof(ParticipantKickViewModel))]
public partial class ParticipantKickModal : ModalViewBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantKickModal"/>
    /// </summary>
    public ParticipantKickModal()
    {
        InitializeComponent();
    }
}
