using MIN.Messaging.Contracts.Messages;
using MIN.Messaging.Contracts.Models.Entities;

namespace MIN.Messaging.Contracts.Models.RoomRelated.ParticipantRelated;

/// <summary>
/// Уведомление о выходе участника из комнаты
/// </summary>
public sealed class ParticipantLeftMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ParticipantLeft;

    /// <inheritdoc />
    public override bool RequiresEncryption => true;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Информация о покинувшем участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;
}
