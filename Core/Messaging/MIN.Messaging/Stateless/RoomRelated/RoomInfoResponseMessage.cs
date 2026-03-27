using MIN.Entities;
using MIN.Messaging.Contracts;

namespace MIN.Messaging.Stateless.RoomRelated;

/// <summary>
/// Ответ с детальной информацией о комнате
/// </summary>
public class RoomInfoResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomInfoResponse;

    /// <inheritdoc />
    public override bool RequiresEncryption => true;

    /// <summary>
    /// Информация о комнате
    /// </summary>
    public Room Room { get; set; } = null!;
}
