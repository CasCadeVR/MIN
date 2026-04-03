using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.RoomRelated.ParticipantRelated;

/// <summary>
/// Уведомление о присоединении нового участника к комнате.
/// </summary>
public sealed class ParticipantJoinedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ParticipantJoined;

    /// <summary>
    /// Идентификатор комнаты.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Информация о присоединившемся участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;
}
