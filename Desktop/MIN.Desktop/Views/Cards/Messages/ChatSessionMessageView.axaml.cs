using MIN.Desktop.ViewModels.Cards.Messages.Sessions;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Cards.Messages;

/// <summary>
/// Сообщение сессии
/// </summary>
public partial class ChatSessionMessageView : CardViewBase<ChatSessionMessageViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatSessionMessageView"/>
    /// </summary>
    public ChatSessionMessageView()
    {
        InitializeComponent();
    }
}
