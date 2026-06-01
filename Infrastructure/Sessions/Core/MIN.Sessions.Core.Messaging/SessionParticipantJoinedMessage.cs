using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.Contracts;

/// <summary>
/// Сообщение о присоединении участника к сессии
/// </summary>
public class SessionParticipantJoinedMessage : BaseMessage, IDescribable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionParticipantJoined;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <inheritdoc />
    public override bool RequiresLocalDuplication => true;

    /// <summary>
    /// Информация об участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    string IDescribable.GetDescription() => $"Игрок {Participant.Name} присоединился к этой сессии";
}
