using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.Contracts.Models;

/// <summary>
/// Сообщение ответа на присоединение к сессии
/// </summary>
public abstract class SessionJoinResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag { get; }

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Флаг, указывающий, нужно ли оповещать остальных
    /// </summary>
    public bool NeedToAnnounce { get; set; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }
}
