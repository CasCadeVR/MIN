namespace MIN.Core.Streaming.Contracts.Events;

/// <summary>
/// Событие успешной сборки сообщения из пакетов
/// </summary>
public sealed class MessageAssembledEventArgs : EventArgs
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
    /// Идентификатор комнаты
    /// </summary>
    public required Guid ConnectionId { get; init; }

    /// <summary>
    /// Собранные данные (для не-raw потоков)
    /// </summary>
    public byte[]? Data { get; init; }

    /// <summary>
    /// Путь к временному файлу с собранными данными (для raw payload)
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// Указывает, что данные являются сырым байтовым потоком (не JSON-сообщение)
    /// </summary>
    public bool IsRawPayload { get; init; }
}
