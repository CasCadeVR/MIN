using MIN.Desktop.ViewModels.Cards.Messages.Files;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Cards.Messages;

/// <summary>
/// Файловое сообщение превью изображения
/// </summary>
public partial class ChatFileImagePreviewMessageView : CardViewBase<ChatFileImagePreviewMessageViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileImagePreviewMessageView"/>
    /// </summary>
    public ChatFileImagePreviewMessageView()
    {
        InitializeComponent();
    }
}
