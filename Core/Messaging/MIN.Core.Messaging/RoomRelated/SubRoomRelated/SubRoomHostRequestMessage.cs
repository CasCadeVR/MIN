using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;
using MIN.Core.SubRooms.Contracts.Enums;

namespace MIN.Core.Messaging.RoomRelated.SubRoomRelated;

/// <summary>
/// Сообщение запроса на хостинг подкомнаты
/// </summary>
public sealed class SubRoomHostRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SubRoomHostRequest;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Цель подкомнаты
    /// </summary>
    public SubRoomPurpose Purpose { get; set; }
}
