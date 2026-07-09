using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели чата
/// </summary>
public partial class ChatSideBarViewModel : RoutableViewModelBase
{
    private ParticipantInfo localParticipant = null!;
    private Guid roomId;

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.RightSideBar;

    /// <inheritdoc />
    public override bool RelatedToCentral => true;

    /// <inheritdoc />
    public override EventHandler? OnNavigatedTo { get; }

    /// <inheritdoc />
    public override EventHandler? OnNavigatedFrom { get; }

    /// <summary>
    /// Комната
    /// </summary>
    [ObservableProperty]
    public partial Room Room { get; set; } = null!;

    /// <summary>
    /// IP Адрес
    /// </summary>
    [ObservableProperty]
    public partial string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Порт
    /// </summary>
    [ObservableProperty]
    public partial string Port { get; set; } = string.Empty;

    /// <summary>
    /// Кабинет
    /// </summary>
    [ObservableProperty]
    public partial string Classroom { get; set; } = string.Empty;

    /// <summary>
    /// Открыта ли панель
    /// </summary>
    public bool IsOpened { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatSideBarViewModel"/>
    /// </summary>
    public ChatSideBarViewModel()
    {
        OnNavigatedTo = (sender, e) => IsOpened = true;
        OnNavigatedFrom = (sender, e) =>
        {
            if (sender is ChatViewModel || sender is ChatSideBarViewModel)
            {
                IsOpened = false;
            }
        };
    }

    /// <summary>
    /// Подгрузить данные о комнате и перезагрузить страницу
    /// </summary>
    public async Task LoadRoomDataAndRefresh(Room room, ParticipantInfo localParticipant)
    {
        this.localParticipant = localParticipant;
        Room = room;
        roomId = room.Id;

        if (IpAddressParser.TryParseIpAddress(room.ConnectionAddress, out var gottenIpAddress, out var port))
        {
            Port = port.ToString();
            IpAddress = gottenIpAddress;
        }

        Classroom = string.IsNullOrEmpty(room.Cabinet) ? DesktopConstants.UndefinedPcName : room.Cabinet;
    }

    /// <summary>
    /// Закрыть страницу
    /// </summary>
    [RelayCommand]
    public void CloseAsync()
    {
        CloseView();
    }
}
