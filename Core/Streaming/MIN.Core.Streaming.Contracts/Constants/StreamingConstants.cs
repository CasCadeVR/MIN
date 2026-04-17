using MIN.Core.Headers.Contracts.Constants;
using MIN.Core.Transport.Contracts.Constants;

namespace MIN.Core.Streaming.Contracts.Constants;

/// <summary>
/// Конфигурация для Streaming
/// </summary>
public class StreamingConstants
{
    /// <summary>
    /// Дефолтный таймаут ожидания пакета (мс)
    /// </summary>
    public const int DefaultChunkTimeoutMs = 1_000;

    /// <summary>
    /// Дефолтное время жизни потока (мс)
    /// </summary>
    public const int DefaultStreamLifetimeMs = 5_000;

    /// <summary>
    /// Размер данных одного пакета (без заголовка)
    /// </summary>
    public const int ChunkDataSize = TransportConstants.MessageBufferSize - HeadersConstants.StreamHeaderSize - HeadersConstants.EncryptionHeaderSize;

    /// <summary>
    /// Размер пакета подтверждения (маркер + StreamId + chunkIndex)
    /// </summary>
    public const int ChunkAckSize = 1 + 16 + 4;
}
