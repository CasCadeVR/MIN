using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.OutOfSubRoom;

/// <summary>
/// Сообщение об уходе участника из сессии
/// </summary>
public class SessionParticipantLeftMessage : BaseMessage, IDescribable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionParticipantLeft;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Информация об участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    string IDescribable.GetDescription() => $"Игрок {Participant.Name} покинул эту сессию";
}
