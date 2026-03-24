using MIN.Messaging.Contracts;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.RoomRelated.ParticipantRelated;

namespace MIN.Messaging.Stateless.RoomRelated;

/// <summary>
/// Ответ с детальной информацией о комнате
/// </summary>
public class RoomInfoResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomInfoResponse;

    /// <inheritdoc />
    public override bool RequiresEncryption => true;

    /// <summary>
    /// Информация о комнате
    /// </summary>
    public RoomInfo Room { get; set; } = null!;

    /// <summary>
    /// Подключённые участники
    /// </summary>
    public List<ParticipantInfo> Participants { get; set; } = [];

    /// <summary>
    /// Последние сообщения
    /// </summary>
    public List<ChatTextMessage> RecentMessages { get; set; } = [];
}
