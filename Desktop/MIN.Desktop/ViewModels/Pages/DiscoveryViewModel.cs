using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
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
    private readonly IDialogService dialogService;

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.Central;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryViewModel"/>
    /// </summary>
    public DiscoveryViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    /// <summary>
    /// Обработчкик создания комнаты
    /// </summary>
    [RelayCommand]
    public async Task CreateRoom()
    {
        var result = await dialogService.ShowAsync<CreateRoomViewModel>();
        if (result == null || result == false)
        {
            return;
        }

        try
        {

            // Don't add to servers list manually here, it will be added by file system watcher. Otherwise: possible duplicate entries by race-condition.
        }
        catch
        {
            //LauncherNotifier.Error($"Server create failed: {ex.Message}");
        }
    }
}
