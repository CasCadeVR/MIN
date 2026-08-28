using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.Stateless.RoomRelated.History;

/// <summary>
/// Сообщение запроса на подгрузку истории сообщений
/// </summary>
public class ChatHistoryResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChatHistoryResponse;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <inheritdoc />
    public override bool RequireStreamAcks => true;

    /// <summary>
    /// Полученые сообщения
    /// </summary>
    public List<IMessage> Messages { get; set; } = [];

    /// <summary>
    /// Количество полученных сообщений
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// С какого времени подгружены сообщения
    /// </summary>
    public DateTime? OldestTimestamp { get; set; }

    /// <summary>
    /// Идентификатор самого последнего сообщения
    /// </summary>
    public Guid? OldestMessageId { get; set; }
}
