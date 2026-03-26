using MIN.Events.Base;

namespace MIN.Events.Events;

/// <summary>
/// Событие, возникающее при изменении статуса подключения к комнате
/// </summary>
public sealed class ConnectionStatusChangedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Флаг подключения
    /// </summary>
    public bool IsConnected { get; init; }

    /// <summary>
    /// Сообщение об ошибке при отключении
    /// </summary>
    public string? ErrorMessage { get; init; }
}
