using MIN.Entities.Contracts.Entities;
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
    /// Участник отправитель
    /// </summary>
    public ParticipantInfo Sender { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MessageReceivedEventArgs"/>
    /// </summary>
    public MessageReceivedEventArgs(Guid roomId, Guid connectionId, IMessage message, ParticipantInfo sender)
    {
        RoomId = roomId;
        ConnectionId = connectionId;
        Message = message;
        Sender = sender;
    }
}
