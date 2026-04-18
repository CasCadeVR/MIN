namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Входящий пакет
/// </summary>
public sealed class PendingChunk
{
    /// <summary>
    /// Индекс пакета
    /// </summary>
    public int LastAcknowledgedIndex { get; set; }

    /// <summary>
    /// Всего пакетов
    /// </summary>
    public int TotalChunks { get; init; }
}
