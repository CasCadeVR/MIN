using MIN.Core.Events.Contracts;

namespace MIN.Core.Events.Events;

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
    /// Идентификатор подключения выходящего участника
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// Флаг подключения
    /// </summary>
    public bool IsConnected { get; init; }

    /// <summary>
    /// Флаг, указывающий, можно ли продолжать общение с комнатой
    /// </summary>
    public bool NeedToDisconnect { get; init; }

    /// <summary>
    /// Сообщение об ошибке при отключении
    /// </summary>
    public string? LeavingMessage { get; init; }
}
