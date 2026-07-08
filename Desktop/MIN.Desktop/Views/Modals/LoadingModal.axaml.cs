using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Modals;

/// <summary>
/// Всплывающее окно создания комнаты
/// </summary>
[ModalForViewModel(typeof(LoadingViewModel))]
public partial class LoadingModal : ModalViewBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LoadingModal"/>
    /// </summary>
    public LoadingModal()
    {
        InitializeComponent();
    }
}
