using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Chess.Messaging;

/// <summary>
/// Сообщение готовности шахмат
/// </summary>
public sealed class ChessReadyMessage : SessionReadyMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessReady;
}
