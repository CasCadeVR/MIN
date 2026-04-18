namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Ключ идентификации пакета среди других приходящих пакетов
/// </summary>
public readonly record struct ChunkAckKey
{
    /// <summary>
    /// Идентфикатор потока
    /// </summary>
    public required Guid StreamId { get; init; }

    /// <summary>
    /// Индекс пакета
    /// </summary>
    public required int ChunkIndex { get; init; }
}
