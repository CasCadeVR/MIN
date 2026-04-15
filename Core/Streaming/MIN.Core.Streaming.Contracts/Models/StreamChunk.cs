namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Пакет потока данных
/// </summary>
public readonly record struct StreamChunk
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public required Guid StreamId { get; init; }

    /// <summary>
    /// Флаги пакета
    /// </summary>
    public required StreamChunkFlags Flags { get; init; }

    /// <summary>
    /// Индекс текущего пакета
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// Общее количество пакетов
    /// </summary>
    public required int Total { get; init; }

    /// <summary>
    /// Данные пакета
    /// </summary>
    public required ReadOnlyMemory<byte> Data { get; init; }

    /// <summary>
    /// Является ли это первым и последним пакетом одновременно (одно сообщение)
    /// </summary>
    public bool IsSingle => Flags.HasFlag(StreamChunkFlags.Start) && Flags.HasFlag(StreamChunkFlags.End);
}
