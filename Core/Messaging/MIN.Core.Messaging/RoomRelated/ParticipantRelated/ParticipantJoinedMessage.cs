using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.RoomRelated.ParticipantRelated;

/// <summary>
/// Уведомление о присоединении нового участника к комнате
/// </summary>
public sealed class ParticipantJoinedMessage : BaseMessage, IDescribable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ParticipantJoined;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Информация о присоединившемся участнике
    /// </summary>
    public Participant Participant { get; set; } = null!;

    string IDescribable.GetDescription() => $"Участник {Participant.Name} зашёл в комнату";
}
