using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

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
    /// Идентификатор сессии
    /// </summary>
    public required string SessionId { get; set; }

    /// <summary>
    /// Версия установленной у клиента сессии
    /// </summary>
    public required Version SessionVersion { get; set; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    /// <remarks>
    /// null, если хостинг впервые, иначе - id неактивной подкомнаты
    /// </remarks>
    public int? SubRoomId { get; set; }
}
