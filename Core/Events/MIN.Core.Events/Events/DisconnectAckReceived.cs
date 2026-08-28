using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении подтверждения на то, что получатель уведомлён об отключении
/// </summary>
public sealed record DisconnectAckReceived : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор участника
    /// </summary>
    public Guid ParticipantId { get; init; }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorMessage { get; init; } = string.Empty;
}
