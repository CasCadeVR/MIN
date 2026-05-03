using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.RoomRelated.ParticipantRelated;

/// <summary>
/// Уведомление о присоединении нового участника к комнате
/// </summary>
public sealed class ParticipantAcceptedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ParticipantAccepted;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }
}
