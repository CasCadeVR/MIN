using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Modals;

/// <summary>
/// Всплывающее окно создания комнаты
/// </summary>
[ModalForViewModel(typeof(CreateRoomViewModel))]
public partial class CreateRoomModal : ModalViewBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CreateRoomModal"/>
    /// </summary>
    public CreateRoomModal()
    {
        InitializeComponent();
    }
}
