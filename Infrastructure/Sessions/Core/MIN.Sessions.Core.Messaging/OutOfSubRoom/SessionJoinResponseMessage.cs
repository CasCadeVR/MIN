using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;
using MIN.Sessions.Core.Messaging.Contracts.Enums;

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
    /// Тип сессии
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Флаг, указывающий, нужно ли оповещать остальных
    /// </summary>
    public bool NeedToAnnounce { get; set; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }
}
