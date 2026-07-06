using System;
using System.Collections.Generic;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Models.ReferenceCommands;
using MIN.Desktop.Infrastructure.Extensions;
using MIN.Desktop.ViewModels.Base;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Extensions;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели
/// </summary>
public partial class MainSideBarViewModel : RoutableViewModelBase
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly SettingsSideBarViewModel settingsSideBarViewModel;
    private readonly DiscoveryViewModel discoveryViewModel;
    private readonly Dictionary<Guid, ChatViewModel> activeChatViews = [];
    private readonly ParticipantInfo localParticipant;

    /// <summary>
    /// Последние комнаты
    /// </summary>
    [ObservableProperty]
    public partial AvaloniaList<RecentRoomCardViewModel> RecentRooms { get; set; } = [];

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.LeftSideBar;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainSideBarViewModel"/>
    /// </summary>
    public MainSideBarViewModel(IMinFeatureCollection featureCollection,
        SettingsSideBarViewModel settingsSideBarViewModel,
        DiscoveryViewModel discoveryViewModel)
    {
        this.featureCollection = featureCollection;
        this.settingsSideBarViewModel = settingsSideBarViewModel;
        this.discoveryViewModel = discoveryViewModel;

        localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();

        this.RegisterMessageListener<RegisterRoomReferenceCommand, MainSideBarViewModel>(static (message, vm)
           => vm.RegisterChat(message.Room, message.View));
    }

    /// <summary>
    /// Открыть настройки
    /// </summary>
    [RelayCommand]
    public void OpenDiscoveryViewAsync() => ChangeView(discoveryViewModel);

    /// <summary>
    /// Открыть настройки
    /// </summary>
    [RelayCommand]
    public void OpenSettingsViewAsync() => ChangeView(settingsSideBarViewModel);

    /// <summary>
    /// Зарегистрировать чат
    /// </summary>
    public void RegisterChat(RoomInfo roomInfo, ChatViewModel panel)
    {
        var roomId = roomInfo.Id;
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(roomId);

        activeChatViews[roomId] = panel;

        RecentRooms.Add(new RecentRoomCardViewModel(featureCollection.Core.EventBus,
            context, roomInfo, localParticipant.Id == roomInfo.HostParticipant.Id));

        //var card = new RecentRoomCard(featureCollection.Core.EventBus,
        //    context,
        //    roomInfo,
        //    AsCreator: roomInfo.HostParticipant.Id == localParticipant.Id)
        //{
        //    Width = flowLayoutPanelRooms.Width - flowLayoutPanelRooms.Margin.Horizontal * 2,
        //};

        //card.Clicked += () =>
        //{
        //    if (selectedRecentRoomCard != card)
        //    {
        //        SelectChatCard(card);
        //        navigationService.NavigateToExisting(panel);
        //    }
        //};
        //flowLayoutPanelRooms.Controls.Add(card);
        //activeRecentRoomCards[roomId] = card;
        //SelectChatCard(card);
    }

    /// <summary>
    /// Удалить чат
    /// </summary>
    public void UnregisterChat(Guid roomId)
    {
        activeChatViews.Remove(roomId);

        //if (activeRecentRoomCards.Remove(roomId, out var card))
        //{
        //    card.Dispose();
        //}
    }

    /// <summary>
    /// Получить view чата
    /// </summary>
    public ChatViewModel? GetChatView(Guid roomId)
      => activeChatViews.TryGetValue(roomId, out var chatPanelView) ? chatPanelView : null;
}
