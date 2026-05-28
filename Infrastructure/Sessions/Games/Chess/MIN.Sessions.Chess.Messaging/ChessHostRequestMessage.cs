using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Chess.Messaging;

/// <summary>
/// Сообщение запроса на хостинг шахмат
/// </summary>
public sealed class ChessHostRequestMessage : SessionHostRequestMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessHostRequest;
}
