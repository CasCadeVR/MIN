using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Core.Messaging;

/// <summary>
/// Сообщение о выходе из сессии
/// </summary>
public sealed class SessionLeaveMessage : BaseSessionMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionLeave;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }
}
