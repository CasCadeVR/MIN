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
    private readonly bool isNew;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [NotifyDataErrorInfo]
    [Required]
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
    public RoomInfo Room { get; set; } = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CreateRoomViewModel"/>
    /// </summary>
    public CreateRoomViewModel(RoomInfo? room = null)
    {
        isNew = room == null;
        Room = new RoomInfo();

        if (!isNew)
        {
            Room = new RoomInfo(room!);
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
