using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.Stateless.RoomRelated.Join;

/// <summary>
/// Ответ на запрос о присоединения к комнате
/// </summary>
public sealed class RoomJoinResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomJoinResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификтор комнаты
    /// </summary>
    public Guid RoomId { get; set; }
}
