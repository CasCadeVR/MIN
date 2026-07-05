using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;
using MIN.DI.FeatureCollection;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели
/// </summary>
public partial class MainSideBarViewModel : RoutableViewModelBase, IChatViewManager
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly SettingsSideBarViewModel settingsSideBarViewModel;
    private readonly DiscoveryViewModel discoveryViewModel;
    //private readonly Dictionary<Guid, RecentRoomCard> activeRecentRoomCards = [];
    private readonly Dictionary<Guid, ChatViewModel> activeChatViews = [];

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

    void IChatViewManager.RegisterChat(RoomInfo roomInfo, ChatViewModel panel)
    {
        var roomId = roomInfo.Id;
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(roomId);

        activeChatViews[roomId] = panel;

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

    /// <inheritdoc />
    public void UnregisterChat(Guid roomId)
    {
        activeChatViews.Remove(roomId);

        //if (activeRecentRoomCards.Remove(roomId, out var card))
        //{
        //    card.Dispose();
        //}
    }

    ChatViewModel? IChatViewManager.GetChatView(Guid roomId)
      => activeChatViews.TryGetValue(roomId, out var chatPanelView) ? chatPanelView : null;
}
