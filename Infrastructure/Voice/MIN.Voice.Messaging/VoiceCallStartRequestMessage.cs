using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Voice.Messaging;

/// <summary>
/// Сообщение запроса на создания звонка хостом
/// </summary>
public sealed class VoiceCallStartRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.VoiceCallStartRequest;

    /// <inheritdoc />
    public override bool IsPublic => false;
}
