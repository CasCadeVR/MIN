using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts.Models;

namespace MIN.Sessions.Core.Messaging.OutOfSubRoom;

/// <summary>
/// Сообщение о закрытии сервера сессии
/// </summary>
public sealed class SessionServerShutdownMessage : BaseSessionMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionServerShutdown;

    /// <summary>
    /// Причина закрытия
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
