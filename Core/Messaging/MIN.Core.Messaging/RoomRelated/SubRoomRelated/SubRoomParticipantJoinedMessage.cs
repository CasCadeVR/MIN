using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.RoomRelated.SubRoomRelated;

/// <summary>
/// Сообщение о присоединении участника к подкомнате
/// </summary>
public sealed class SubRoomParticipantJoinedMessage : BaseMessage, IDescribable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SubRoomParticipantJoined;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    /// <summary>
    /// Информация об участние
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;

    string IDescribable.GetDescription() => $"Участник {Participant.Name} присоединился к активности #{SubRoomId}";
}
