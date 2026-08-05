using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.Stateless.RoomRelated.Ping;

/// <summary>
/// Проверка доступности (ответ)
/// </summary>
public sealed class PongMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.Pong;

    /// <inheritdoc />
    public override bool IsPublic => false;
}
