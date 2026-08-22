namespace MIN.Core.Streaming.Contracts.Events.Sending;

/// <summary>
/// Событие успешной отправки всего потока сообщений
/// </summary>
public sealed class StreamCompletedEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public required Guid StreamId { get; init; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public required Guid RoomId { get; init; }

    /// <summary>
    /// Указывает, что данные являются сырым байтовым потоком (не JSON-сообщение)
    /// </summary>
    public bool IsRawPayload { get; init; }
}
