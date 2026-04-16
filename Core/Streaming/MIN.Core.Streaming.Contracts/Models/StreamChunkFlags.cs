namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Флаги для пакетов, регулирующие поток
/// </summary>
[Flags]
public enum StreamChunkFlags : byte
{
    /// <summary>
    /// Обычное сообщение
    /// </summary>
    RegularMessage = 0x00,

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
    Mid = 0x04,

    /// <summary>
    /// Флаг, указывающий что отправитель будет ждать подтверждения получения пакета
    /// </summary>
    RequiresAcks = 0x08,

    /// <summary>
    /// Байт-маркер для подтверждения чанка (ACK)
    /// </summary>
    Ack = 0x80,
}
