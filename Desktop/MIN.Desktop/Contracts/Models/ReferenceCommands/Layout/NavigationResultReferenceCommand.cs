using MIN.Desktop.ViewModels.Pages;
using MIN.Desktop.ViewModels.Pages.ChatViewModels;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;

/// <summary>
/// Команда передачи экземпляра ChatVM для восстановления
/// </summary>
public sealed class NavigationResultReferenceCommand(ChatViewModel chatVm, MainSideBarViewModel sideBar)
{
    /// <summary>
    /// View model главной Chat
    /// </summary>
    public ChatViewModel ChatVm { get; } = chatVm;

    /// <summary>
    /// View model главной sideBar
    /// </summary>
    public MainSideBarViewModel SideBar { get; } = sideBar;
}

