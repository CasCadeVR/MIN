namespace MIN.Core.Transport.Contracts.Models.Constants;

/// <summary>
/// Конфигурация для Transport
/// </summary>
public static class TransportConstants
{
    /// <summary>
    /// Размер буффера для сообщений
    /// </summary>
    public const int MessageBufferSize = 1024 * 64;

    /// <summary>
    /// Максимально возможное количество соединений
    /// </summary>
    public const int RoomMaximumConnectionsAmount = 254;

    /// <summary>
    /// Размер заголовка потокового пакета (1 байт флагов + 16 байт GUID + 4 байта индекса + 4 байта количества)
    /// </summary>
    public const int StreamHeaderSize = 25;

    /// <summary>
    /// Дефолтный таймаут ожидания пакета (мс)
    /// </summary>
    public const int DefaultChunkTimeoutMs = 10_000;

    /// <summary>
    /// Дефолтное время жизни потока (мс)
    /// </summary>
    public const int DefaultStreamLifetimeMs = 60_000;

    /// <summary>
    /// Размер данных одного пакета (без заголовка)
    /// </summary>
    public const int ChunkDataSize = MessageBufferSize - StreamHeaderSize;

    /// <summary>
    /// Байт-маркер для подтверждения чанка (ACK)
    /// </summary>
    public const byte ChunkAckMarker = 0x80;

    /// <summary>
    /// Размер пакета подтверждения (маркер + StreamId + chunkIndex)
    /// </summary>
    public const int ChunkAckSize = 1 + 16 + 4;
}
