using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Infrastructure.Validators;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна создания комнаты
/// </summary>
public partial class CreateRoomViewModel : ModalViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [NotifyDataErrorInfo]
    [Required]
    [RoomName]
    [NotEndsWith(".")]
    public partial string Name { get; set; } = "";

    /// <summary>
    /// Настраиваемая комната
    /// </summary>
    public Room Room { get; set; } = null!;

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private void Create()
    {
        Room.Name = Name;
        Close(ButtonOptions.Ok);
    }

    private bool CanCreate() => !HasErrors;
}
