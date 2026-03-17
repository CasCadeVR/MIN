using MIN.Messaging.Contracts.Messages;

namespace MIN.Messaging.Contracts.Models.Stateless.RoomRelated;

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
    public Guid? RoomId { get; set; }
}
