namespace MIN.Core.Streaming.Contracts.Events;

/// <summary>
/// Событие запроса на отправку ACK
/// </summary>
public sealed class ChunkReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid StreamId { get; init; }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// Идентфикатор соелинения сервера
    /// </summary>
    public Guid? ServerConnectionId { get; init; }

    /// <summary>
    /// Сколько уже загрузилось
    /// </summary>
    public long ReceivedBytes { get; init; }
}
