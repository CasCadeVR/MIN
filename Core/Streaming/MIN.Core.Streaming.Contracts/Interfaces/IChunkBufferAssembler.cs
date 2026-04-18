using MIN.Core.Streaming.Contracts.Events;

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
    event EventHandler<ChunkReceivedEventArgs>? ChunkReceived;

    /// <summary>
    /// Обрабатывает входящий пакет
    /// </summary>
    Task ProcessStreamChunk(byte[] data, Guid connectionId, Guid roomId, CancellationToken cancellationToken = default);
}
