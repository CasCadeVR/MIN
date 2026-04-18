namespace MIN.Core.Headers.Contracts.Enums;

/// <summary>
/// Флаги заголовка для типа сообщений
/// </summary>
[Flags]
public enum HeaderMessageType : byte
{
    /// <summary>
    /// Незашифрованое
    /// </summary>
    Plain = 0x00,

    /// <summary>
    /// Зашифрованое
    /// </summary>
    Encrypted = 0x01,

    /// <summary>
    /// Пакет потока
    /// </summary>
    StreamChunk = 0x10,

    /// <summary>
    /// Ответ получения пакета потока
    /// </summary>
    Ack = 0x80
}
