using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts.Models;

namespace MIN.Sessions.Chess.Messaging.Default;

/// <summary>
/// Сообщение ответа на присоединение к подкомнате
/// </summary>
public sealed class ChessJoinResponseMessage : SessionJoinResponseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessJoinResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Текущая ситуация на доске
    /// </summary>
    public string CurrentPositionOnBoard { get; set; } = string.Empty;
}
