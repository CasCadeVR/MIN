using MIN.Core.Stores.Contracts.Models;

namespace MIN.Core.Handlers.Contracts.Models;

/// <summary>
/// Контекст обработки сообщения
/// </summary>
public sealed class MessageContext
{
    /// <summary>
    /// Контекст комнаты, в которой было получено сообщение
    /// </summary>
    public RoomContext RoomContext { get; init; }

    /// <summary>
    /// Идентификатор соеднинения, по которому пришло сообщение
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// Токен отмены для длительных операций
    /// </summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Инициализирует новый экзмепляр <see cref="MessageContext"/>
    /// </summary>
    public MessageContext(RoomContext roomContext, Guid connectionId, CancellationToken cancellationToken)
    {
        RoomContext = roomContext;
        ConnectionId = connectionId;
        CancellationToken = cancellationToken;
    }
}
