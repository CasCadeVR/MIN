using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Chess.Messaging.Default;

/// <summary>
/// Сообщение о присоединении участника к шахматам
/// </summary>
public sealed class ChessParticipantJoinedMessage : SessionParticipantJoinedMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessParticipantJoined;
}
