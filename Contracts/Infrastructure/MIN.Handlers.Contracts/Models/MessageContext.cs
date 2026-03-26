using MIN.Messaging.Contracts.Entities;

namespace MIN.Handlers.Contracts.Models;

/// <summary>
/// Контекст обработки сообщения
/// </summary>
public sealed class MessageContext
{
    /// <summary>
    /// Отправитель сообщения
    /// </summary>
    public ParticipantInfo Sender { get; init; } = null!;

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
    public MessageContext(ParticipantInfo sender, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
    {
        Sender = sender;
        RoomId = roomId;
        ConnectionId = connectionId;
        CancellationToken = cancellationToken;
    }
}
