using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Models;

namespace MIN.Core.Streaming.Contracts.Interfaces;

/// <summary>
/// Ассемблер пакетов в цельное сообщение
/// </summary>
public interface IChunkBufferAssembler
{
    /// <summary>
    /// Событие завершения сборки сообщения
    /// </summary>
    event EventHandler<MessageAssembledEventArgs>? MessageAssembled;

    /// <summary>
    /// Событие запроса на отправку ACK
    /// </summary>
    event EventHandler<ChunkAckRequestedEventArgs>? ChunkAckRequested;

    /// <summary>
    /// Обрабатывает входящий пакет
    /// </summary>
    void AddChunk(StreamChunk chunk, Guid connectionId, Guid roomId);

    /// <summary>
    /// Обрабатывает входящий пакет асинхронно
    /// </summary>
    Task AddChunkAsync(StreamChunk chunk, Guid connectionId, Guid roomId, CancellationToken cancellationToken = default);
}
