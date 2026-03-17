using MIN.Messaging.Contracts.Messages;
using MIN.Messaging.Contracts.Models.Entities;

namespace MIN.Messaging.Contracts.Models.RoomRelated.ParticipantRelated;

/// <summary>
/// Уведомление о присоединении нового участника к комнате.
/// </summary>
public sealed class ParticipantJoinedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ParticipantJoined;

    /// <inheritdoc />
    public override bool RequiresEncryption => true;

    /// <summary>
    /// Идентификатор комнаты.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Информация о присоединившемся участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;
}
