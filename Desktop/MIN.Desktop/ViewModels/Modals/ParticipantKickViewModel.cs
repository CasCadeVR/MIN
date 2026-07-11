using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна создания комнаты
/// </summary>
public partial class ParticipantKickViewModel : ModalViewModelBase
{
    /// <summary>
    /// Имя участника
    /// </summary>
    [ObservableProperty]
    public partial string ParticipantName { get; set; } = "";

    /// <summary>
    /// Причина кика
    /// </summary>
    [ObservableProperty]
    public partial string Reason { get; set; } = "";

    [RelayCommand]
    private void Kick()
    {
        Close(ButtonOptions.Ok);
    }
}
