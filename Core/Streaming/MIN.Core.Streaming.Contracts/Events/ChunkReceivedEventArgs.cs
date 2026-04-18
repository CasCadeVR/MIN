namespace MIN.Core.Streaming.Contracts.Events;

/// <summary>
/// Событие запроса на отправку ACK
/// </summary>
public sealed class ChunkReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public required Guid StreamId { get; init; }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public required Guid ConnectionId { get; init; }

    /// <summary>
    /// Идентфикатор комнаты
    /// </summary>
    public required Guid RoomId { get; init; }

    /// <summary>
    /// Номер пакета
    /// </summary>
    public required int ChunkIndex { get; init; }
}
