using MIN.Core.Messaging.Contracts;
using MIN.Core.SubRooms.Contracts.Interfaces.Messages;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;

namespace MIN.Sessions.Chess.Messaging.Default;

/// <summary>
/// Сообщение об уходе участника из шахмат
/// </summary>
public sealed class ChessParticipantLeftMessage : SessionParticipantLeftMessage, IWithinSubRoom
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessParticipantLeft;
}
