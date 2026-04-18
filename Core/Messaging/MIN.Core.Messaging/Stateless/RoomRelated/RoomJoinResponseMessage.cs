using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.Stateless.RoomRelated;

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

    /// <summary>
    /// Разрешение на вход
    /// </summary>
    public bool Allow { get; set; }
}
