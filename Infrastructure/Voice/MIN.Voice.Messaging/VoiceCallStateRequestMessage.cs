using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Voice.Messaging;

/// <summary>
/// Сообщение запрос на состояния звонка в комнате
/// </summary>
public sealed class VoiceCallStateRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.VoiceStateRequest;

    /// <inheritdoc />
    public override bool IsPublic => false;
}
