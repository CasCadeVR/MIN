using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.Input;
using MIN.Chat.Messaging;
using MIN.Desktop.Infrastructure.Services;

namespace MIN.Desktop.ViewModels.Cards.Messages;

/// <summary>
/// Текстовое сообщение участника
/// </summary>
public partial class ChatTextMessageViewModel : BaseChatMessageViewModel
{
    private readonly IClipboard? clipboard;

    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    public ChatTextMessage ChatMessage { get; init; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatTextMessageViewModel"/>
    /// </summary>
    public ChatTextMessageViewModel(ChatTextMessage chatMessage,
        Thickness timePadding,
        bool isLocal,
        bool isHostMessage,
        bool removeHeaders,
        IClipboard? clipboard)
        : base(chatMessage.Sender.Name,
            chatMessage.Timestamp,
            timePadding,
            isLocal,
            isHostMessage,
            removeHeaders,
            chatMessage.RecipientId != null)
    {
        ChatMessage = chatMessage;
        this.clipboard = clipboard;
    }

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        if (clipboard != null)
        {
            await clipboard.SetTextAsync(ChatMessage.Content);
            InAppNotifier.Info("Скопировано в буфер обмена");
        }
    }
}
