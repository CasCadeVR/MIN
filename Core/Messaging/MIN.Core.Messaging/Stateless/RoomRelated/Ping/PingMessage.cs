using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.Stateless.RoomRelated.Ping;

/// <summary>
/// Проверка доступности (запрос)
/// </summary>
public sealed class PingMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.Ping;

    /// <inheritdoc />
    public override bool IsPublic => false;
}
