using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging;

/// <summary>
/// Сообщение ошибки на запрос присоединения к сессии
/// </summary>
public sealed class SessionJoinFailedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionJoinFailed;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
