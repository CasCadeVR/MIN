namespace MIN.Core.Services.Contracts.Models;

/// <summary>
/// Флаги заголовка для типа сообщений
/// </summary>
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
