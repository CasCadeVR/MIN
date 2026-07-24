using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace MIN.Desktop.ViewModels.Cards.Messages;

/// <summary>
/// Системное сообщение
/// </summary>
public partial class SystemChatMessageViewModel : BaseChatMessageViewModel
{
    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Событие по нажатию на системное сообщение
    /// </summary>
    public Func<Task>? OnClicked { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SystemChatMessageViewModel"/>
    /// </summary>
    public SystemChatMessageViewModel() : base() { }

    [RelayCommand]
    private async Task Click()
    {
        var task = OnClicked?.Invoke();
        task ??= Task.CompletedTask;
        await task;
    }
}
