using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;
using MIN.Core.SubRooms.Contracts.Enums;

namespace MIN.Sessions.Core.Messaging.Contracts;

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
    /// Цель подкомнаты
    /// </summary>
    public SubRoomPurpose Purpose => SubRoomPurpose.Activity;
}
