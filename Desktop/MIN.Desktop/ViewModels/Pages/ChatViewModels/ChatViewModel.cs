using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;
using MIN.DI.FeatureCollection;

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
    private readonly CancellationTokenSource appCts = new();
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
            localParticipant = featureCollection.Core.IdentityService.SelfParticipant.ToParticipantInfo();

            OnNavigatedTo = ActionOnNavigatedTo;
            OnNavigatedFrom = ActionOnNavigatedFrom;

            InitializeNotifications();
            InitializeTimers();
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
        SubscribeToEvents(featureCollection.Core.EventBus);

        await UpdateChatFlow();

        if (!IsHost)
        {
            await RequestVoiceCallStateAsync();
        }
        else
        {
            loadingTcs.SetResult();
        }
    }

    private async Task CleanUpServicesAsync(Guid roomId, Guid connectionId)
    {
        if (localParticipant.Id == room.HostParticipant.Id)
        {
            await featureCollection.Discovery.DiscoveryService.StopDiscoveryAsync(roomId);
            await featureCollection.Core.Lifecycle.StopHostingAsync(roomId);
        }
        else
        {
            await featureCollection.Core.Lifecycle.DisconnectAsync(roomId, connectionId, DisconnectReason.None);
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
        roomScope.Dispose();
        errorToken.Dispose();
        typingTimer.Dispose();
        appCts.Cancel();
        appCts.Dispose();
        await CleanUpServicesAsync(roomId, connectionId);
    }
}
