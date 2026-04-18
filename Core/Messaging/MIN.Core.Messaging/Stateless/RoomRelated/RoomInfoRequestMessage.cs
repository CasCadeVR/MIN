using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.Stateless.RoomRelated;

/// <summary>
/// Запрос информации о комнате
/// </summary>
public sealed class RoomInfoRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomInfoRequest;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификтор комнаты
    /// </summary>
    public Guid RoomId { get; set; }
}
