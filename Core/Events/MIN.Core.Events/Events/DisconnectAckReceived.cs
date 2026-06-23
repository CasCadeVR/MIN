using MIN.Core.Events.Contracts;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при получении подтверждения на то, что получатель уведомлён об отключении
/// </summary>
public sealed class DisconnectAckReceived : BaseEvent
{
    /// <summary>
    /// Идентификатор участника
    /// </summary>
    public Guid ParticipantId { get; init; }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorMessage { get; init; } = string.Empty;
}
