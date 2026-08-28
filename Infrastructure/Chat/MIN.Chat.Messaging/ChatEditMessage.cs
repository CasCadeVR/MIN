using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Chat.Messaging;

/// <summary>
/// Действие удаления сообщение из чата
/// </summary>
public sealed class ChatEditMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.MessageEdit;

    /// <inheritdoc />
    public override bool RequiresLocalDuplication => true;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Идентификатор удаляемого сообщения
    /// </summary>
    public Guid MessageIdToEdit { get; set; }

    /// <summary>
    /// Новый текстовый контент для сообщения
    /// </summary>
    public string NewContent { get; set; } = string.Empty;
}
