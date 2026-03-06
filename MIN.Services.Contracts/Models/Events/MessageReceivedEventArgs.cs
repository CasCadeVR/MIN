using MIN.Services.Contracts.Models.Messages;

namespace MIN.Services.Contracts.Models.Events;

/// <summary>
/// Аргументы события получения сообщения
/// </summary>
public class MessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Полученное сообщение
    /// </summary>
    public ChatMessage Message { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MessageReceivedEventArgs"/>
    /// </summary>
    public MessageReceivedEventArgs(ChatMessage сhatMessage)
    {
        Message = сhatMessage;
    }
}
