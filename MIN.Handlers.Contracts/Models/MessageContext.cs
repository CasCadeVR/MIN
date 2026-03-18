using MIN.Messaging.Contracts.Entities;
using MIN.Transport.Contracts;

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
    /// Транспорт, через который можно отправить ответные сообщения
    /// </summary>
    public ITransport? Transport { get; init; }

    /// <summary>
    /// Токен отмены для длительных операций
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
