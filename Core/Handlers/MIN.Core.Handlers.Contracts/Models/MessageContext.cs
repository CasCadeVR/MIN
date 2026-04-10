namespace MIN.Core.Handlers.Contracts.Models;

/// <summary>
/// Контекст обработки сообщения
/// </summary>
public sealed class MessageContext
{
    /// <summary>
    /// Идентфикатор отправителя сообщения
    /// </summary>
    public Guid SenderId { get; init; }

    /// <summary>
    /// Идентификатор комнаты, в которой было получено сообщение
    /// </summary>
    public Guid RoomId { get; init; }

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
    public MessageContext(Guid senderId, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
    {
        SenderId = senderId;
        RoomId = roomId;
        ConnectionId = connectionId;
        CancellationToken = cancellationToken;
    }
}
