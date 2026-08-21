namespace MIN.Core.Streaming.Contracts.Events;

/// <summary>
/// Событие ошибки при передачи данных потока
/// </summary>
public sealed class StreamFailedEventArgs : EventArgs
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
    /// Что произошло
    /// </summary>
    public string? ErrorMessage { get; init; }
}
