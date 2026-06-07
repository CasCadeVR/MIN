using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.Contracts.Models;

/// <summary>
/// Сообщение запроса на присоединение к сессии
/// </summary>
public abstract class SessionJoinRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag { get; }

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }
}
