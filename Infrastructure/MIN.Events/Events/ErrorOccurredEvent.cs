using MIN.Events.Base;

namespace MIN.Events.Events;

/// <summary>
/// Событие, возникающее при ошибке в работе комнаты или соединения
/// </summary>
public sealed class ErrorOccurredEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты (если применимо)
    /// </summary>
    public Guid? RoomId { get; init; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorMessage { get; init; } = string.Empty;
}
