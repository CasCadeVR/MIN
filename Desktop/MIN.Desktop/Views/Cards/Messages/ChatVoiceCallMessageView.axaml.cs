using MIN.Desktop.ViewModels.Cards.Messages.Voice;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Views.Cards.Messages;

/// <summary>
/// Сообщение сессии
/// </summary>
public partial class ChatVoiceCallMessageView : CardViewBase<ChatVoiceCallMessageViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatVoiceCallMessageView"/>
    /// </summary>
    public ChatVoiceCallMessageView()
    {
        InitializeComponent();
    }
}
