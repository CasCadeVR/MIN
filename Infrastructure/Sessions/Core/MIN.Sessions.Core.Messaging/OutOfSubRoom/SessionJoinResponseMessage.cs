using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.OutOfSubRoom;

/// <summary>
/// Сообщение ответа на присоединение к сессии
/// </summary>
public sealed class SessionJoinResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionJoinResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор сессии
    /// </summary>
    public required string SessionId { get; set; }

    /// <summary>
    /// Флаг, указывающий, нужно ли оповещать остальных
    /// </summary>
    public bool NeedToAnnounce { get; set; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }
}
