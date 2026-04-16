using MIN.Core.Services.Contracts.Models;
using MIN.Core.Streaming.Contracts.Models;

namespace MIN.Core.Services.Contracts.Interfaces.ByteSerialization;

/// <summary>
/// Сервис по работе с заголовками пакетов данных
/// </summary>
public interface IHeaderManager
{
    /// <summary>
    /// Добавить заголовок шифрования
    /// </summary>
    byte[] AddEncryptionHeader(byte[] secretData);

    /// <summary>
    /// Добавить пустой заголовок
    /// </summary>
    byte[] AddPlainHeader(byte[] plainData);

    /// <summary>
    /// Проверяет по заголовку, зашифровано ли сообщение
    /// </summary>
    bool IsEncrypted(byte[] data);

    /// <summary>
    /// Убрать заголовок шифрования
    /// </summary>
    byte[] RemoveEncryptionHeader(byte[] data);

    /// <summary>
    /// Получить тип сообщения из заголовка
    /// </summary>
    HeaderMessageType GetMessageType(byte[] data);

    /// <summary>
    /// Проверяет по заголовку, является ли сообщение пакетом потока
    /// </summary>
    bool IsStreamChunk(byte[] data);

    /// <summary>
    /// Получить заголовки пакета потока
    /// </summary>
    StreamChunkFlags GetStreamFlags(byte[] data);

    /// <summary>
    /// Создать заголовок для пакета потока
    /// </summary>
    byte[] BuildStreamChunkHeader(StreamChunkFlags flags, Guid streamId, int index, int total);

    /// <summary>
    /// Распарсить заголовок пакета потока
    /// </summary>
    StreamChunkHeader ParseStreamChunkHeader(byte[] data);
}
