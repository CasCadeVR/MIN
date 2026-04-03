using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.RoomRelated.ParticipantRelated;

/// <summary>
/// Уведомление о выходе участника из комнаты
/// </summary>
public sealed class ParticipantLeftMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ParticipantLeft;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Информация о покинувшем участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;
}
