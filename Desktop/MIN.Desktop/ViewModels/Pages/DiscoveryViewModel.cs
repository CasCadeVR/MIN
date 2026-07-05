using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Modals;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель обнаружения комнат
/// </summary>
public partial class DiscoveryViewModel : RoutableViewModelBase
{
    private readonly ChatViewModel chatViewModel;
    private readonly IDialogService dialogService;

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.Central;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryViewModel"/>
    /// </summary>
    public DiscoveryViewModel(ChatViewModel chatViewModel, IDialogService dialogService)
    {
        this.chatViewModel = chatViewModel;
        this.dialogService = dialogService;
    }

    /// <summary>
    /// Обработчик создания комнаты
    /// </summary>
    [RelayCommand]
    public async Task CreateRoom()
    {
        var result = await dialogService.ShowDialogAsync<CreateRoomViewModel>();
        if (result == null)
        {
            return;
        }

        var newRoomInfo = new RoomInfo(result.Room);
        //chatViewManager.RegisterChat(newRoomInfo, chatView);
        ChangeView(chatViewModel);

        try
        {

            // Don't add to servers list manually here, it will be added by file system watcher. Otherwise: possible duplicate entries by race-condition.
        }
        catch
        {
            //LauncherNotifier.Error($"Server create failed: {ex.Message}");
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
            //await OnRoomJoin(directConnectForm.Endpoint);
            result.EnableConnectButton();
        };
    }
}
