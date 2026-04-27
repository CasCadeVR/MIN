using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.Stateless.RoomRelated;

/// <summary>
/// Ответ на то, что получатель уведомлён о причине отказа присоединения к комнате
/// </summary>
public sealed class RoomJoinRejectedAckMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.RoomJoinRejectAck;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор сообщения отказа
    /// </summary>
    public Guid RejectionMessageId { get; set; }

    /// <summary>
    /// Причина отказа
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
