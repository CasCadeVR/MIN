using MIN.Core.Messaging.Contracts;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Core.Messaging;

/// <summary>
/// Сообщение об ошибке внутри сессии
/// </summary>
public sealed class SessionErrorMessage : BaseSessionMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionError;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Критична ли ошибка (нужно ли выйти)
    /// </summary>
    public bool Critical { get; set; }
}
