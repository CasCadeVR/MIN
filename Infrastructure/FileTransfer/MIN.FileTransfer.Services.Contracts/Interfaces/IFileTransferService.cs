using MIN.FileTransfer.Services.Contracts.Models;
using MIN.FileTransfer.Services.Contracts.Models.Enums;

namespace MIN.FileTransfer.Services.Contracts.Interfaces;

/// <summary>
/// Сервис по работе с файлами
/// </summary>
public interface IFileTransferService
{
    /// <summary>
    /// Получить путь к папке комнаты
    /// </summary>
    string GetRoomDirectory(Guid roomId);

    /// <summary>
    /// Получить путь к файлу
    /// </summary>
    string? GetFilePath(Guid roomId, string fileName);

    /// <summary>
    /// Сохранить файл
    /// </summary>
    Task<string> SaveFileAsync(Guid roomId, string fileName, Stream fileStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Открыть файл для чтения
    /// </summary>
    Task<Stream?> OpenFileForReadingAsync(Guid roomId, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Очистить файлы комнаты
    /// </summary>
    Task DeleteRoomFilesAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Регистрирует информацию о передаче файла
    /// </summary>
    void RegisterTransfer(Guid transferId, Guid roomId, FileTransferDirection direction, string fileName);

    /// <summary>
    /// Зарегистрировать входящий файл
    /// </summary>
    void RegisterPendingMetadata(Guid transferId, string fileName);

    /// <summary>
    /// Попытаться получить имя входящего файла 
    /// </summary>
    bool TryGetPendingFileName(Guid transferId, out string fileName);

    /// <summary>
    /// Попытаться получить информацию о передаче файла
    /// </summary>
    bool TryGetTransferInfo(Guid transferId, out TransferInfo info);

    /// <summary>
    /// Удалить передачу файла из списка текущих передач
    /// </summary>
    void RemoveTransfer(Guid transferId);

    /// <summary>
    /// Удалить файл
    /// </summary>
    Task DeleteFileAsync(Guid roomId, string fileName, CancellationToken cancellationToken = default);
}
