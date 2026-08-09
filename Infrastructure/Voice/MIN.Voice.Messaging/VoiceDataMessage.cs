using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Enums;
using MIN.Core.Messaging.Contracts.Messages;
using MIN.Core.SubRooms.Contracts.Interfaces.Messages;

namespace MIN.Voice.Messaging;

/// <summary>
/// Сообщение для передачи звуковых данных
/// </summary>
public sealed class VoiceDataMessage : BaseMessage, IWithinSubRoom
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.VoiceData;

    /// <inheritdoc />
    public override MessageChannel Channel => MessageChannel.Fast;

    /// <inheritdoc />
    public int SubRoomId { get; set; }

    /// <summary>
    /// Содержимое сообщения (звук)
    /// </summary>
    public required byte[] Voice { get; set; }
}
