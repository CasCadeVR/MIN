using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Voice.Messaging;

/// <summary>
/// Сообщения подтверждения входа звонка
/// </summary>
public sealed class VoiceCallEstablishedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.VoiceCallEstablished;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    /// <summary>
    /// Флаг, указывающий, нужно ли оповещать остальных
    /// </summary>
    public bool NeedToAnnounce { get; set; }
}
