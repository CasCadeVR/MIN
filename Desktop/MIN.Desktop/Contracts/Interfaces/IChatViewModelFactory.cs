using MIN.Desktop.ViewModels.Pages.ChatViewModels;

namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Фабрика по предоставлению <see cref="ChatViewModel"/>
/// </summary>
public interface IChatViewModelFactory
{
    /// <summary>
    /// Создать <see cref="ChatViewModel"/>
    /// </summary>
    ChatViewModel Create();
}
