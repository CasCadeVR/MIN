using MIN.Core.Entities;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.Stateless.RoomRelated.RoomInfo;

/// <summary>
/// Ответ с детальной информацией о комнате
/// </summary>
public sealed class RoomInfoResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomInfoResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <inheritdoc />
    public override bool RequireStreamAcks => true;

    /// <summary>
    /// Информация о комнате
    /// </summary>
    public Room Room { get; set; } = null!;
}
