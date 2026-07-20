using MIN.Desktop.ViewModels.Cards.Messages;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Cards.Messages;

/// <summary>
/// Системное сообщение
/// </summary>
public partial class SystemChatMessageView : CardViewBase<SystemChatMessageViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SystemChatMessageView"/>
    /// </summary>
    public SystemChatMessageView()
    {
        InitializeComponent();
    }
}
