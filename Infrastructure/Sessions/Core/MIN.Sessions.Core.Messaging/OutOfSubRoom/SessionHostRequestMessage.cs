using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;
using MIN.Sessions.Core.Messaging.Contracts.Enums;

namespace MIN.Sessions.Core.Messaging.OutOfSubRoom;

/// <summary>
/// Сообщение запроса на хостинг сессии
/// </summary>
public sealed class SessionHostRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionHostRequest;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Тип сессии
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    /// <remarks>
    /// null, если хостинг впервые, иначе - id неактивной подкомнаты
    /// </remarks>
    public int? SubRoomId { get; set; }
}
