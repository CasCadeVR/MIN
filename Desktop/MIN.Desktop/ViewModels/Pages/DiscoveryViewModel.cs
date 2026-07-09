using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models.ReferenceCommands;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Modals;
using MIN.DI.FeatureCollection;
using MIN.Discovery.Events;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель обнаружения комнат
/// </summary>
public partial class DiscoveryViewModel : RoutableViewModelBase
{
    private readonly IChatViewModelFactory chatViewModelFactory;
    private readonly IMinFeatureCollection featureCollection;
    private readonly IDialogService dialogService;
    private readonly CancellationTokenSource lifeTimeCts = null!;
    private readonly ParticipantInfo localParticipant = null!;
    private CancellationTokenSource? discoveryCts;

    private Settings Settings => featureCollection.Helper.SettingsProvider.GetSettings();

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.Central;

    /// <summary>
    /// Идёт ли сейчас процесс обнаружения
    /// </summary>
    [ObservableProperty]
    public partial bool isDiscovering { get; set; }

    /// <summary>
    /// Обнаруженные комнаты
    /// </summary>
    [ObservableProperty]
    public partial AvaloniaList<DiscoveredRoomCardViewModel> DiscoveredRooms { get; set; } = [];

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryViewModel"/>
    /// </summary>
    public DiscoveryViewModel(IChatViewModelFactory chatViewModelFactory,
        IMinFeatureCollection featureCollection,
        ICtsProvider ctsProvider,
        IDialogService dialogService)
    {
        this.chatViewModelFactory = chatViewModelFactory;
        this.featureCollection = featureCollection;
        this.dialogService = dialogService;

        if (!Design.IsDesignMode)
        {
            localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();

            lifeTimeCts = ctsProvider.AppCts;
            SubscribeToEvents();
        }
    }

    private void SubscribeToEvents()
    {
        featureCollection.Core.EventBus.Subscribe<RoomDiscoveredEvent>(OnRoomDiscovered);
    }

    private async Task<bool> ResolveParticipant()
    {
        if (Settings.DefaultParticipantName != string.Empty)
        {
            localParticipant.Name = Settings.DefaultParticipantName;
            featureCollection.Helper.IdentityService.SetParticipant(localParticipant);
        }
        else
        {
            var participantCreatingResult = await dialogService.ShowDialogAsync<CreateParticipantViewModel>();
            if (participantCreatingResult != null && participantCreatingResult == false)
            {
                return false;
            }

            Settings.DefaultParticipantName = featureCollection.Helper.IdentityService.SelfParticipant.Name;
            featureCollection.Helper.SettingsProvider.SaveSettings(Settings);
        }
        return true;
    }

    /// <summary>
    /// Обработчик создания комнаты
    /// </summary>
    [RelayCommand]
    public async Task CreateRoom()
    {
        var createViewModelResult = await dialogService.ShowDialogAsync<CreateRoomViewModel>();
        if (createViewModelResult! == false)
        {
            return;
        }

        if (!await ResolveParticipant())
        {
            return;
        }

        var roomInfo = createViewModelResult!.Room;
        var roomId = roomInfo.Id;

        var chatViewModel = chatViewModelFactory.Create();
        ChangeView(chatViewModel);

        try
        {
            var room = await featureCollection.Core.RoomHoster.StartHostingAsync(roomInfo, createViewModelResult.RoomAutoPortForward, lifeTimeCts.Token);
            await featureCollection.Discovery.DiscoveryService.StartDiscoveryAsync(roomId, lifeTimeCts.Token);

            await chatViewModel.LoadRoomDataAndRefresh(room, CoreRegistryConstants.LocalConnectionId);
            RegisterRoom(roomInfo, chatViewModel);

            InAppNotifier.Success($"Комната {room.Name} успешно создана!");
        }
        catch (Exception ex)
        {
            InAppNotifier.Error($"Не удалось создать комнату: {ex.Message}");
        }
    }

    private static void RegisterRoom(RoomInfo roomInfo, ChatViewModel chatViewModel)
    {
        WeakReferenceMessenger.Default.Send(new RegisterRoomReferenceCommand(roomInfo, chatViewModel));
    }

    /// <summary>
    /// Обработчик обнаружения комнат
    /// </summary>
    [RelayCommand]
    public void DiscoverRooms()
    {
        if (isDiscovering)
        {
            discoveryCts?.Cancel();
        }
        else
        {
            _ = PerformDiscovery();
        }
    }

    private async Task PerformDiscovery()
    {
        isDiscovering = true;
        DiscoveredRooms.Clear();
        discoveryCts = CancellationTokenSource.CreateLinkedTokenSource(lifeTimeCts.Token);

        try
        {
            await featureCollection.Discovery.DiscoveryService.DiscoverRoomsAsync(
                TimeSpan.FromMilliseconds(Settings.DiscoveryTimeout), discoveryCts.Token);
        }
        catch (Exception ex)
        {
            InAppNotifier.Error($"Ошибка обнаружения: {ex.Message}");
        }
        finally
        {
            isDiscovering = false;
        }
    }

    private async Task OnRoomDiscovered(RoomDiscoveredEvent e, CancellationToken cancellationToken)
    {
        foreach (var discoveryInfo in e.RoomDiscoveryInfos)
        {
            var card = new DiscoveredRoomCardViewModel(featureCollection.Core.EventBus,
                discoveryInfo.Room,
                localParticipant.Id == discoveryInfo.Room.HostParticipant.Id);

            card.Clicked += async () => await OnRoomJoin(discoveryInfo.Endpoint, discoveryInfo.Room, card);

            DiscoveredRooms.Add(card);
        }
    }

    private async Task OnRoomJoin(IEndpoint endpoint, RoomInfo? roomInfo = null, DiscoveredRoomCardViewModel? card = null)
    {
        if (roomInfo != null && featureCollection.Core.RoomConnector.IsConnected(roomInfo.Id))
        {
            InAppNotifier.Info("Вы уже подключены к этой комнате");
            card?.IsConnecting = false;
            return;
        }

        if (!await ResolveParticipant())
        {
            return;
        }

        var connectCts = CancellationTokenSource.CreateLinkedTokenSource(lifeTimeCts.Token);
        LoadingViewModel? loadingVm = null;

        try
        {
            ConnectionResult connectionResult = new();

            await dialogService.ShowAsync<LoadingViewModel>(async vm =>
            {
                await vm.LoadRoomDataAndRefresh(async room =>
                    {
                        if (room == null)
                        {
                            return;
                        }
                        var newRoomInfo = new RoomInfo(room);

                        var chatViewModel = chatViewModelFactory.Create();
                        ChangeView(chatViewModel);

                        await chatViewModel.LoadRoomDataAndRefresh(room, connectionResult.ConnectionId);
                        RegisterRoom(newRoomInfo, chatViewModel);
                    }, connectCts, DesktopConstants.RoomConnectionTimeoutMs);

                loadingVm = vm;
            });

            connectionResult = await featureCollection.Core.RoomConnector.ConnectAsync(endpoint, connectCts.Token);

            loadingVm?.RoomId = connectionResult.RoomId;
        }
        catch (Exception ex)
        {
            loadingVm?.CloseByCode();
            InAppNotifier.Error($"Произошла ошибка при подключении: {ex.Message}");
        }
        finally
        {
            card?.IsConnecting = false;
        }
    }

    /// <summary>
    /// Обработчик подключения напрямую
    /// </summary>
    [RelayCommand]
    public async Task ConnectDirectly()
    {
        var result = await dialogService.ShowAsync<DirectConnectViewModel>();
        if (result == null)
        {
            return;
        }

        result.OnConnect += async () =>
        {
            await OnRoomJoin(result.Endpoint);
            result.EnableConnectButton();
        };
    }
}
