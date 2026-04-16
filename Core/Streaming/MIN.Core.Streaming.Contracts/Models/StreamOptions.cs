using MIN.Core.Transport.Contracts.Models.Constants;

namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Настройки потока
/// </summary>
public sealed class StreamOptions
{
    /// <summary>
    /// Идентфикатор потока
    /// </summary>
    public Guid StreamId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Нужно ли подтверждать получение пакетов со стороны получателя
    /// </summary>
    public bool RequiresAcks { get; init; }

    /// <summary>
    /// Нужно ли шифровать чанки
    /// </summary>
    public bool RequiresEncryption { get; init; }

    /// <summary>
    /// Таймаут ожидания каждого пакета (мс)
    /// </summary>
    public int ChunkTimeoutMs { get; init; } = TransportConstants.DefaultChunkTimeoutMs;

    /// <summary>
    /// Общее время жизни потока (мс)
    /// </summary>
    public int StreamLifetimeMs { get; init; } = TransportConstants.DefaultStreamLifetimeMs;
}
