using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Chat.Messaging;

/// <summary>
/// Текстовое сообщение чата, отправляемое участником в комнату
/// </summary>
public sealed class ChatTextMessage : BaseContentMessage, IDescribable, IReplyable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChatTextMessage;

    /// <inheritdoc />
    public override bool RequireStreamAcks => true;

    /// <inheritdoc />
    public override bool RequiresLocalDuplication => true;

    /// <summary>
    /// Отправитель сообщения
    /// </summary>
    public ParticipantInfo Sender { get; set; } = null!;

    /// <inheritdoc />
    public string? ReplyToMessageDescription { get; set; }

    string IDescribable.GetDescription() => $"{Sender.Name}: {Content}";
}
