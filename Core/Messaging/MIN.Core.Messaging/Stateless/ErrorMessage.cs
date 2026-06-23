using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.Stateless;

/// <summary>
/// Сообщение об ошибке, передаваемое по сети
/// </summary>
public sealed class ErrorMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.Error;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; set; } = "Произошла ошибка";
}
