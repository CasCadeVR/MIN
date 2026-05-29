using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Chess.Services.Contracts.Models;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Chess.Messaging.Default;

/// <summary>
/// Сообщение запроса на присоединение к подкомнате
/// </summary>
public sealed class ChessJoinRequestMessage : SessionJoinRequestMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessJoinRequest;

    /// <summary>
    /// Настройки присоединения к шахматам
    /// </summary>
    public ChessJoinRequestOptions? Options { get; set; }
}
