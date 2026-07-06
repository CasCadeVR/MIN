using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models.ReferenceCommands;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Modals;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель обнаружения комнат
/// </summary>
public partial class DiscoveryViewModel : RoutableViewModelBase
{
    private readonly ChatViewModel chatViewModel;
    private readonly IMinFeatureCollection featureCollection;
    private readonly IDialogService dialogService;
    private readonly CancellationTokenSource lifeTimeCts;
    private readonly ParticipantInfo localParticipant;

    private Settings Settings => featureCollection.Helper.SettingsProvider.GetSettings();

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.Central;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryViewModel"/>
    /// </summary>
    public DiscoveryViewModel(ChatViewModel chatViewModel,
        IMinFeatureCollection featureCollection,
        ICtsProvider ctsProvider,
        IDialogService dialogService)
    {
        this.chatViewModel = chatViewModel;
        this.featureCollection = featureCollection;
        this.dialogService = dialogService;

        localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();

        lifeTimeCts = ctsProvider.AppCts;
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
            if (participantCreatingResult == null)
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
        if (createViewModelResult == null)
        {
            return;
        }

        if (!await ResolveParticipant())
        {
            return;
        }

        var roomInfo = createViewModelResult.Room;
        var roomId = roomInfo.Id;

        ChangeView(chatViewModel);

        try
        {
            var room = await featureCollection.Core.RoomHoster.StartHostingAsync(roomInfo, createViewModelResult.RoomAutoPortForward, lifeTimeCts.Token);
            await featureCollection.Discovery.DiscoveryService.StartDiscoveryAsync(roomId, lifeTimeCts.Token);

            RegisterRoom(roomInfo);
        }
        catch
        {
            //LauncherNotifier.Error($"Server create failed: {ex.Message}");
        }
    }

    private void RegisterRoom(RoomInfo roomInfo)
    {
        WeakReferenceMessenger.Default.Send(new RegisterRoomReferenceCommand()
        {
            Room = roomInfo,
            View = chatViewModel
        });
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
            //await OnRoomJoin(directConnectForm.Endpoint);
            result.EnableConnectButton();
        };
    }
}
