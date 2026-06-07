using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.Contracts.Models;

/// <summary>
/// Сообщение запроса на хостинг сессии
/// </summary>
public abstract class SessionHostRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag { get; }

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    /// <remarks>
    /// null, если хостинг впервые, иначе - id неактивной подкомнаты
    /// </remarks>
    public int? SubRoomId { get; set; }
}
