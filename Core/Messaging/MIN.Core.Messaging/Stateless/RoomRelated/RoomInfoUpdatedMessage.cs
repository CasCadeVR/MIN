using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.Stateless.RoomRelated;

/// <summary>
/// Обновление информации о комнате
/// </summary>
public sealed class RoomInfoUpdatedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomInfoUpdated;

    /// <inheritdoc />
    public override bool RequiresLocalDuplication => true;

    /// <summary>
    /// Информация о комнате
    /// </summary>
    public RoomInfo Room { get; set; } = null!;
}
