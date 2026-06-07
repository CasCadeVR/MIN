using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts.Models;

namespace MIN.Sessions.Core.Messaging.OutOfSubRoom;

/// <summary>
/// Сообщение о выходе из сессии
/// </summary>
public sealed class SessionLeaveMessage : BaseSessionMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionLeave;

    /// <inheritdoc />
    public override bool IsPublic => false;
}
