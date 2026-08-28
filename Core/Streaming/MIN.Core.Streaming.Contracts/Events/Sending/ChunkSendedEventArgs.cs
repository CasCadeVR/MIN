namespace MIN.Core.Streaming.Contracts.Events.Sending;

/// <summary>
/// Событие на отправления пакета данных
/// </summary>
public sealed class ChunkSendedEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid StreamId { get; init; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public required Guid RoomId { get; init; }

    /// <summary>
    /// Сколько уже загрузилось
    /// </summary>
    public long ReceivedBytes { get; init; }
}
