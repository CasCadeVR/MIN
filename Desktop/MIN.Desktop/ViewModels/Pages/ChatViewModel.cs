using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Extensions;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    private readonly ChatSideBarViewModel chatSideBarViewModel;
    private readonly IDialogService dialogService;

    private readonly IMinFeatureCollection featureCollection;
    private readonly CancellationTokenSource formCts = new();

    private readonly ParticipantInfo localParticipant = null!;
    private Guid roomId;
    private Guid connectionId;
    private Room room = null!;

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.Central;

    /// <summary>
    /// Имя комнаты
    /// </summary>
    [ObservableProperty]
    public partial string RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatViewModel"/>
    /// </summary>
    public ChatViewModel(ChatSideBarViewModel chatSideBarViewModel,
        IMinFeatureCollection featureCollection,
        IDialogService dialogService)
    {
        this.featureCollection = featureCollection;
        this.chatSideBarViewModel = chatSideBarViewModel;
        this.dialogService = dialogService;

        if (!Design.IsDesignMode)
        {
            localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();
        }
    }

    /// <summary>
    /// Подгрузить данные о комнате и перезагрузить страницу
    /// </summary>
    public async Task LoadRoomDataAndRefresh(Room room, Guid connectionId)
    {
        ToggleSideBar();
        this.room = room;
        RoomName = room.Name;
        this.connectionId = connectionId;
        roomId = room.Id;
    }

    /// <summary>
    /// Открыть боковую панель
    /// </summary>
    [RelayCommand]
    public void ToggleSideBar() => ChangeView(chatSideBarViewModel);
}
