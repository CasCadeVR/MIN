using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.RoomRelated.SubRoomRelated;

/// <summary>
/// Сообщение ответа на присоединение к подкомнате
/// </summary>
public sealed class SubRoomJoinResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SubRoomJoinResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    /// <summary>
    /// Успех присоединения подкомнаты
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке в случае неудачи
    /// </summary>
    public string? ErrorMessage { get; set; }
}
