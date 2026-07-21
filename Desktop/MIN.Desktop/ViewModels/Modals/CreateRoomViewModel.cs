using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Infrastructure.Validators;
using MIN.Desktop.ViewModels.Base;
using MIN.Helpers.Services;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна создания комнаты
/// </summary>
public partial class CreateRoomViewModel : ModalViewModelBase
{
    private bool isNew;

    [ObservableProperty]
    [Display(Name = "Имя комнаты")]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Придумайте имя для комнаты")]
    [RoomName]
    [NotEndsWith(".")]
    public partial string Name { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [Range(1, DesktopConstants.MaximumParticipantsInRoom)]
    [NotifyDataErrorInfo]
    public partial int RoomMaxPlayers { get; set; } = 8;

    [ObservableProperty]
    public partial bool RoomAutoPortForward { get; set; }

    /// <summary>
    /// Настраиваемая комната
    /// </summary>
    public RoomInfo Room { get; set; } = new RoomInfo();

    /// <summary>
    /// Инициализироовать с уже созданной комнатой
    /// </summary>
    public void InitializeWithRoom(RoomInfo room)
    {
        isNew = false;

        if (!isNew)
        {
            Room = room;
            Name = room.Name;
            RoomMaxPlayers = room.MaximumParticipants;
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private void Create()
    {
        Room.Name = Name;

        if (CollegePCNameParser.TryParseComputerName(Environment.MachineName, out var roomNumber, out var computerNumber))
        {
            Room.Cabinet = roomNumber.ToString();
            Room.PcNumber = computerNumber;
        }

        Room.MaximumParticipants = RoomMaxPlayers;

        Close(ButtonOptions.Ok);
    }

    private bool CanCreate() => !HasErrors;
}
