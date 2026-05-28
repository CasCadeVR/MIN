using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.Contracts;

/// <summary>
/// Сообщение о присоединении участника к сессии
/// </summary>
public abstract class SessionParticipantJoinedMessage : BaseMessage, IDescribable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag { get; }

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    /// <summary>
    /// Информация об участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;

    string IDescribable.GetDescription() => $"Игрок {Participant.Name} присоединился к этой сессии";
}
