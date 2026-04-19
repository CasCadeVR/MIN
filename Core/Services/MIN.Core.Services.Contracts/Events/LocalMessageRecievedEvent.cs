using MIN.Core.Events.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Services.Contracts.Events;

/// <summary>
/// Аргументы события получения сообщения внутри программы
/// </summary>
public sealed class LocalMessageRecievedEvent : BaseEvent
{
    /// <summary>
    /// Полученное сообщение
    /// </summary>
    public IMessage Message { get; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LocalMessageRecievedEvent"/>
    /// </summary>
    public LocalMessageRecievedEvent(IMessage message, Guid roomId)
    {
        Message = message;
        RoomId = roomId;
    }
}
