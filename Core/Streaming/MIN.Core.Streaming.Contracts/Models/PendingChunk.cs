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
    public int TotalChunks { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="PendingChunk"/>
    /// </summary>
    public PendingChunk(int lastAcknowledged, int totalChunks)
    {
        LastAcknowledgedIndex = lastAcknowledged;
        TotalChunks = totalChunks;
    }
}
