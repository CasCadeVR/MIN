using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.RoomRelated;

/// <summary>
/// Системное сообщение чата, отправляемое для уведомления участников
/// </summary>
public sealed class SystemTextMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SystemMessage;

    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
