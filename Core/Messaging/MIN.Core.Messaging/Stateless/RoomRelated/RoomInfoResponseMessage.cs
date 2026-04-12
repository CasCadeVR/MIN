using MIN.Core.Entities;
using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.Stateless.RoomRelated;

/// <summary>
/// Ответ с детальной информацией о комнате
/// </summary>
public sealed class RoomInfoResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomInfoResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Информация о комнате
    /// </summary>
    public Room Room { get; set; } = null!;
}
