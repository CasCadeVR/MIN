using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Chat.Messaging;

/// <summary>
/// Действие удаления сообщение из чата
/// </summary>
public sealed class ChatDeleteMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.MessageDelete;

    /// <inheritdoc />
    public override bool RequiresLocalDuplication => true;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Идентификатор удаляемого сообщения
    /// </summary>
    public Guid MessageIdToDelete { get; set; }
}
