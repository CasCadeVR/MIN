using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;

namespace MIN.Sessions.Core.Messaging.Contracts;

/// <summary>
/// Сообщение о присоединении участника к сессии
/// </summary>
public abstract class SessionParticipantJoinedMessage : BaseSessionMessage, IDescribable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag { get; }

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Информация об участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;

    string IDescribable.GetDescription() => $"Игрок {Participant.Name} присоединился к этой сессии";
}
