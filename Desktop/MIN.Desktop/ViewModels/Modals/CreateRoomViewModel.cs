using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Transport.Contracts.Models;
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
    [NotifyDataErrorInfo]
    [RoomCapacity]
    public partial int RoomMaxPlayers { get; set; } = 8;

    /// <summary>
    /// Локальное обнаружение
    /// </summary>
    [ObservableProperty]
    public partial bool EnableLocalDiscovery { get; set; }

    /// <summary>
    /// Проброска порта
    /// </summary>
    [ObservableProperty]
    public partial bool EnablePortForwarding { get; set; }

    /// <summary>
    /// Radmin
    /// </summary>
    [ObservableProperty]
    public partial bool EnableRadmin { get; set; }

    /// <summary>
    /// Публикация в web
    /// </summary>
    [ObservableProperty]
    public partial bool EnableWeb { get; set; }

    /// <summary>
    /// Создание или редактирование комнаты
    /// </summary>
    [ObservableProperty]
    public partial bool IsNew { get; set; } = true;

    /// <summary>
    /// Название окна
    /// </summary>
    [ObservableProperty]
    public partial string Title { get; set; } = "MIN - Создание комнаты";

    /// <summary>
    /// Настраиваемая комната
    /// </summary>
    public RoomInfo Room { get; set; } = new RoomInfo();

    /// <summary>
    /// Настройки глобальности сети
    /// </summary>
    public NetworkOptions NetworkOptions { get; set; }

    /// <summary>
    /// Инициализироовать с уже созданной комнатой
    /// </summary>
    public void InitializeWithRoom(RoomInfo room, NetworkOptions networkOptions)
    {
        Title = $"Редактирование комнаты {room.Name}";

        IsNew = false;
        Room = room;
        Name = room.Name;
        RoomMaxPlayers = room.MaximumParticipants;
        NetworkOptions = networkOptions;

        EnableLocalDiscovery = NetworkOptions.EnableLocalDiscovery;
        EnablePortForwarding = NetworkOptions.EnablePortForwarding;
        EnableRadmin = NetworkOptions.EnableRadmin;
        EnableWeb = NetworkOptions.EnableWeb;
    }

    partial void OnEnableRadminChanged(bool value)
    {
        if (EnablePortForwarding && value)
        {
            EnablePortForwarding = false;
        }
    }

    partial void OnEnablePortForwardingChanged(bool value)
    {
        if (EnableRadmin && value)
        {
            EnableRadmin = false;
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

        NetworkOptions = new()
        {
            EnableLocalDiscovery = EnableLocalDiscovery,
            EnablePortForwarding = EnablePortForwarding,
            EnableRadmin = EnableRadmin,
            EnableWeb = EnableWeb,
        };

        Close(ButtonOptions.Ok);
    }

    private bool CanCreate() => !HasErrors;
}
