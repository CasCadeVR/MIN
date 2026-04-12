using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.Stateless.RoomRelated;

/// <summary>
/// Запрос на присоединения к комнате
/// </summary>
public sealed class RoomJoinRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomJoinRequest;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификтор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomInfoRequestMessage"/>
    /// </summary>
    public RoomJoinRequestMessage() { }
}
