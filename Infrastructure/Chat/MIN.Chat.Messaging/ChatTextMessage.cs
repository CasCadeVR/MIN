using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;

namespace MIN.Chat.Messaging;

/// <summary>
/// Текстовое сообщение чата, отправляемое участником в комнату
/// </summary>
public sealed class ChatTextMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChatTextMessage;

    /// <inheritdoc />
    public override bool RequiresEncryption => true;

    /// <summary>
    /// Отправитель сообщения
    /// </summary>
    public ParticipantInfo Sender { get; set; } = null!;

    /// <summary>
    /// Идентификатор комнаты, в которую отправлено сообщение
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор сообщения, на которое дан ответ
    /// </summary>
    public Guid? ReplyToMessageId { get; set; }
}
