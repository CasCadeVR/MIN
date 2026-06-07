using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Chess.Services.Contracts.Models;
using MIN.Sessions.Core.Messaging.Contracts.Models;

namespace MIN.Sessions.Chess.Messaging.Default;

/// <summary>
/// Сообщение запроса на хостинг шахмат
/// </summary>
public sealed class ChessHostRequestMessage : SessionHostRequestMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChessHostRequest;

    /// <summary>
    /// Настройки хостинга шахмат
    /// </summary>
    public ChessHostRequestOptions? Options { get; set; }
}
