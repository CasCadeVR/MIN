using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Chess.Messaging.Default;

/// <summary>
/// Сообщение об уходе участника из шахмат
/// </summary>
public sealed class ChessParticipantLeftMessage : SessionParticipantLeftMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessParticipantLeft;
}
