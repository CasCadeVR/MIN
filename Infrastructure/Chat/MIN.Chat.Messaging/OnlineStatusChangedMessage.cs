using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Chat.Messaging;

/// <summary>
/// Сообщение о смене статуса действия в сети, отправляемое участником в комнату
/// </summary>
public sealed class OnlineStatusChangedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.OnlineStatusChanged;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Статус действия в сети
    /// </summary>
    public OnlineStatus Status { get; set; }
}
