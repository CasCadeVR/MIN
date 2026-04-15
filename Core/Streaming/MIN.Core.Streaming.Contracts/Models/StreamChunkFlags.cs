namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Флаги для пакетов, регулирующие поток
/// </summary>
public enum StreamChunkFlags : byte
{
    /// <summary>
    /// Начало потока
    /// </summary>
    Start = 0x01,

    /// <summary>
    /// Конец потока
    /// </summary>
    End = 0x02,

    /// <summary>
    /// Пакет потока
    /// </summary>
    Mid = 0x04
}
