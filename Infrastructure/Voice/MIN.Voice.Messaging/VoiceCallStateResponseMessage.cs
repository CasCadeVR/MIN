using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Voice.Messaging;

/// <summary>
/// Сообщение ответ на состояния звонка в комнате
/// </summary>
public sealed class VoiceCallStateResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.VoiceStateResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    /// <remarks>
    /// null, если активного звонка неут, иначе - id подкомнаты звонка
    /// </remarks>
    public int? ActiveSubRoomId { get; set; }

    /// <summary>
    /// Когда начался звонок
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Идентификаторы участников в звонке
    /// </summary>
    public List<Guid> CallParticipantIds { get; set; } = [];
}
