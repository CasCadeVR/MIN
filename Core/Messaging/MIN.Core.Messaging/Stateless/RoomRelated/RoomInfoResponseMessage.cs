using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Messaging.Stateless.RoomRelated;

/// <summary>
/// Ответ с детальной информацией о комнате
/// </summary>
public class RoomInfoResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomInfoResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Информация о комнате
    /// </summary>
    public Room Room { get; set; } = null!;

    /// <summary>
    /// Участники комнаты
    /// </summary>
    public List<ParticipantInfo> Participants { get; set; } = new();

    /// <summary>
    /// Последние сообщения
    /// </summary>
    public List<IMessage> RecentMessages { get; set; } = new();
}
