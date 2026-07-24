using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Extensions;

namespace MIN.Desktop.ViewModels.Pages.ChatViewModels;

/// <summary>
/// Модель чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    private readonly ChatSideBarViewModel chatSideBarViewModel;
    private readonly DiscoveryViewModel discoveryViewModel;
    private readonly MainSideBarViewModel mainSideBarViewModel;
    private readonly IDialogService dialogService;

    private readonly IMinFeatureCollection featureCollection;
    private readonly CancellationTokenSource formCts = new();
    private readonly TaskCompletionSource loadingTcs = new();

    private readonly ParticipantInfo localParticipant = null!;
    private Guid roomId;
    private Guid connectionId;
    private Room room = null!;

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.Central;

    /// <inheritdoc />
    public override EventHandler? OnNavigatedTo { get; }

    /// <inheritdoc />
    public override EventHandler? OnNavigatedFrom { get; }

    /// <summary>
    /// Имя комнаты
    /// </summary>
    [ObservableProperty]
    public partial string RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Является ли локальный пользователь хостом комнаты
    /// </summary>
    [ObservableProperty]
    public partial bool IsHost { get; set; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId => roomId;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatViewModel"/>
    /// </summary>
    public ChatViewModel(ChatSideBarViewModel chatSideBarViewModel,
        MainSideBarViewModel mainSideBarViewModel,
        DiscoveryViewModel discoveryViewModel,
        IMinFeatureCollection featureCollection,
        IDialogService dialogService)
    {
        this.featureCollection = featureCollection;
        this.mainSideBarViewModel = mainSideBarViewModel;
        this.discoveryViewModel = discoveryViewModel;
        this.chatSideBarViewModel = chatSideBarViewModel;
        this.dialogService = dialogService;

        if (!Design.IsDesignMode)
        {
            localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();

            OnNavigatedTo = ActionOnNavigatedTo;
            OnNavigatedFrom = ActionOnNavigatedFrom;

            SubscribeToEvents(featureCollection.Core.EventBus);
            InitializeNotifications();
            InitializeTypingTimer();
            InitializeLayoutStyles();
            InitializeParentFormWindowStateEvents();
            InitializeObservableCollections();
        }
    }

    private async void ActionOnNavigatedTo(object? sender, EventArgs e)
    {
        if (chatSideBarViewModel.IsOpened)
        {
            ChangeView(chatSideBarViewModel);
        }

        if (loadedPage == 1)
        {
            return;
        }

        loadedPage = 1;
        renderedMessageCount = 0;
        var lastHistory = featureCollection.Core.RoomFactory
            .GetOrCreateContext(roomId).Messages.GetRecentHistory();

        await RenderMessages(lastHistory.ToList());
        ShowLoadMoreLabel();
    }

    private void ActionOnNavigatedFrom(object? sender, EventArgs e)
    {
        if (loadedPage > 1)
        {
            Messages.Clear();
        }
    }

    /// <inheritdoc />
    public override async Task ViewContentLoadAsync(CancellationToken cancellationToken = default) => await loadingTcs.Task;

    /// <summary>
    /// Подгрузить данные о комнате и перезагрузить страницу
    /// </summary>
    public async Task LoadRoomDataAndRefresh(Room room, Guid connectionId)
    {
        ToggleRightSideBar();
        await chatSideBarViewModel.LoadRoomDataAndRefresh(room, localParticipant);

        this.room = room;
        RoomName = room.Name;
        IsHost = localParticipant.Id == room.HostParticipant.Id;
        this.connectionId = connectionId;
        roomId = room.Id;

        await UpdateChatFlow();
        loadingTcs.SetResult();
    }

    private async Task CleanUpAsync(Guid roomId, Guid connectionId, bool isHost)
    {
        if (isHost)
        {
            await featureCollection.Discovery.DiscoveryService.StopDiscoveryAsync(roomId);
            await featureCollection.Core.RoomHoster.StopHostingAsync(roomId);
        }
        else
        {
            await featureCollection.Core.RoomConnector.DisconnectAsync(roomId, connectionId);
        }
    }

    private async Task Disconnect()
    {
        await DisposeAsync();
        ChangeView(discoveryViewModel);
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync()
    {
        ClearParentFormEvents();
        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
        typingTimer.Dispose();
        formCts.Cancel();
        formCts.Dispose();
        await CleanUpAsync(roomId, connectionId, isHost: localParticipant.Id == room.HostParticipant.Id);
    }
}
