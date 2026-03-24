using MIN.Messaging.Contracts.Interfaces;

namespace MIN.Messaging.Contracts.Events;

/// <summary>
/// Аргументы события получения сообщения
/// </summary>
public class MessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid ConnectionId { get; }

    /// <summary>
    /// Полученное сообщение
    /// </summary>
    public IMessage Message { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MessageReceivedEventArgs"/>
    /// </summary>
    public MessageReceivedEventArgs(Guid roomId, Guid connectionId, IMessage message)
    {
        RoomId = roomId;
        ConnectionId = connectionId;
        Message = message;
    }
}
