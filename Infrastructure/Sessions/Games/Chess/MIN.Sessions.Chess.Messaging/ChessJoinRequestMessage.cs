using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Chess.Messaging;

/// <summary>
/// Сообщение запроса на присоединение к подкомнате
/// </summary>
public sealed class ChessJoinRequestMessage : SessionJoinRequestMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessJoinRequest;
}
