using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Models;
using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Services.Contracts.Events;

/// <summary>
/// Аргументы события получения сообщения внутри программы
/// </summary>
public sealed record LocalMessageReceivedEvent : BaseEvent
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
    /// Роль пользователя
    /// </summary>
    public Role Role { get; }

    /// <summary>
    /// Список исключённых из Broadcast для сервера
    /// </summary>
    public IEnumerable<Guid>? BroadcastExcludeIds { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LocalMessageReceivedEvent"/>
    /// </summary>
    public LocalMessageReceivedEvent(IMessage message, Guid roomId, Role role, IEnumerable<Guid>? broadcastExcludeIds = null)
    {
        Message = message;
        RoomId = roomId;
        Role = role;
        BroadcastExcludeIds = broadcastExcludeIds;
    }
}
