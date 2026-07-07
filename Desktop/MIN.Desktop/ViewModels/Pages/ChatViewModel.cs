using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    private readonly ChatSideBarViewModel chatSideBarViewModel;
    private readonly IDialogService dialogService;

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.Central;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatViewModel"/>
    /// </summary>
    public ChatViewModel(ChatSideBarViewModel chatSideBarViewModel, IDialogService dialogService)
    {
        this.chatSideBarViewModel = chatSideBarViewModel;
        this.dialogService = dialogService;
    }

    /// <summary>
    /// Подгрузить данные о комнате и перезагрузить страницу
    /// </summary>
    public async Task LoadRoomDataAndRefresh()
    {
        ToggleSideBar();
    }

    /// <summary>
    /// Открыть боковую панель
    /// </summary>
    [RelayCommand]
    public void ToggleSideBar() => ChangeView(chatSideBarViewModel);
}
