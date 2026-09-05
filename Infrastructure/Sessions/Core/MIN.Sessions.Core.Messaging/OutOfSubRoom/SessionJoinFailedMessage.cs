using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.OutOfSubRoom;

/// <summary>
/// Сообщение ошибки на присоединение к сессии
/// </summary>
public sealed class SessionJoinFailedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionJoinFailed;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }
}
