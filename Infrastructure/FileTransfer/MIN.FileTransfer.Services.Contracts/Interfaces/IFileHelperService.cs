namespace MIN.FileTransfer.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для получения информации о файлах
/// </summary>
public interface IFileHelperService
{
    /// <summary>
    /// Получить MIME-тип файла по расширению
    /// </summary>
    string GetMimeType(string fileName);

    /// <summary>
    /// Получить размер файла в байтах
    /// </summary>
    long GetFileSize(string filePath);

    /// <summary>
    /// Проверить, допустим ли размер файла
    /// </summary>
    bool IsFileSizeAllowed(long fileSize);

    /// <summary>
    /// Проверить, допустимо ли расширение файла
    /// </summary>
    bool IsExtensionAllowed(string fileName);

    /// <summary>
    /// Очистить имя файла от опасных символов
    /// </summary>
    string SanitizeFileName(string fileName);

    /// <summary>
    /// Форматировать размер файла в читаемый вид (байт, КБ, МБ, ГБ)
    /// </summary>
    string FormatFileSize(long bytes);
}
