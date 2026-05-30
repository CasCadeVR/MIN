using MIN.Core.Events.Contracts;
using MIN.Sessions.Core.Messaging;

namespace MIN.Sessions.Core.Events;

/// <summary>
/// Получена информация о готовой сессии в комнате
/// </summary>
public sealed class SessionReadyMessageReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Полученная информация о сессии
    /// </summary>
    public SessionReadyMessage Message { get; init; } = null!;
}
