using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.RoomRelated.SubRoomRelated;

/// <summary>
/// Сообщение ответа на хостинг подкомнаты
/// </summary>
public sealed class SubRoomHostResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SubRoomHostResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    /// <summary>
    /// Успех хостинга подкомнаты
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке в случае неудачи
    /// </summary>
    public string? ErrorMessage { get; set; }
}
