using MIN.FileTransfer.Services.Contracts.Models;

namespace MIN.FileTransfer.Services.Contracts.Interfaces;

/// <summary>
/// Сервис координации передачи файлов
/// </summary>
public interface IFileTransferService
{
    /// <summary>
    /// Зарегистрировать информацию о передаче файла
    /// </summary>
    void RegisterTransfer(TransferInfo info);

    /// <summary>
    /// Попытаться получить информацию о передаче файла
    /// </summary>
    bool TryGetTransferInfo(Guid transferId, out TransferInfo info);

    /// <summary>
    /// Удалить передачу файла из списка текущих передач
    /// </summary>
    void RemoveTransfer(Guid transferId);

    /// <summary>
    /// Зарегистрировать входящие метаданные файла
    /// </summary>
    void RegisterPendingMetadata(Guid transferId, string fileName);

    /// <summary>
    /// Попытаться получить имя входящего файла
    /// </summary>
    bool TryGetPendingFileName(Guid transferId, out string fileName);

    /// <summary>
    /// Удалить зарегистрированные метаданные
    /// </summary>
    void RemovePendingMetadata(Guid transferId);

    /// <summary>
    /// Зарегистрировать информацию о файле по Id сообщения метаданных
    /// </summary>
    void RegisterFileMetadata(Guid fileMetadataId, Guid roomId, string fileName, string? originalFilePath = null);

    /// <summary>
    /// Получить информацию о файле по Id сообщения метаданных
    /// </summary>
    bool TryGetFileMetadata(Guid fileMetadataId, out FileMetadataInfo info);

    /// <summary>
    /// Начать приём файла из потока (вызывается при сборке чанков)
    /// </summary>
    Task OnFileDataReceivedAsync(Guid transferId, byte[] data, CancellationToken cancellationToken = default);
}
