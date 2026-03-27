using MIN.Messaging.Contracts;

namespace MIN.Messaging.Stateless.RoomRelated;

/// <summary>
/// Запрос информации о комнате
/// </summary>
public class RoomInfoRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomInfoRequest;

    /// <inheritdoc />
    public override bool RequiresEncryption => true;

    /// <summary>
    /// Идентификтор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomInfoRequestMessage"/>
    /// </summary>
    public RoomInfoRequestMessage(Guid roomId)
    {
        RoomId = roomId;
    }
}
