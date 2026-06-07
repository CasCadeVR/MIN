using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Sessions.Core.Messaging.OutOfSubRoom;

/// <summary>
/// Сообщение ошибки на хостинг подкомнаты
/// </summary>
public sealed class SessionHostFailedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionHostFailed;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
