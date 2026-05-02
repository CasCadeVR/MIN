namespace MIN.FileTransfer.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для хранения файлов на диске
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Получить путь к папке комнаты
    /// </summary>
    string GetRoomDirectory(Guid roomId);

    /// <summary>
    /// Убедиться, что папка комнаты существует
    /// </summary>
    void EnsureRoomDirectoryExists(Guid roomId);

    /// <summary>
    /// Проверить, существует ли файл в папке комнаты
    /// </summary>
    bool FileExists(Guid roomId, string fileName);

    /// <summary>
    /// Получить полный путь к файлу, если он существует
    /// </summary>
    string? GetFilePath(Guid roomId, string fileName);

    /// <summary>
    /// Сохранить файл
    /// </summary>
    /// <remarks>
    /// Если файл с таким именем уже существует, добавляет _1, _2 и т.д.
    /// </remarks>
    /// <returns>Итоговое имя файла</returns>
    Task<string> SaveFileAsync(Guid roomId, Stream source, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Открыть файл для чтения
    /// </summary>
    Task<Stream?> OpenFileForReadingAsync(Guid roomId, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить файл из папки комнаты
    /// </summary>
    Task DeleteFileAsync(Guid roomId, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить все файлы комнаты
    /// </summary>
    Task DeleteRoomFilesAsync(Guid roomId, CancellationToken cancellationToken = default);
}
