using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.Stateless.RoomRelated.History;

/// <summary>
/// Сообщение запроса на подгрузку истории сообщений
/// </summary>
public sealed class ChatHistoryRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChatHistoryRequest;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <inheritdoc />
    public override bool RequireStreamAcks => true;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Страница
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Размер страницы
    /// </summary>
    public int PageSize { get; set; } = 25;
}
