using MIN.FileTransfer.Messaging;

namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для работы с файлами в чате
/// </summary>
public interface IChatFileService
{
    /// <summary>
    /// Отправить файл
    /// </summary>
    Task SendFileAsync(Guid roomId, string content, string fileName, string filePath, Guid? recipientId = null, Guid? replyToId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запросить загрузку файла
    /// </summary>
    Task RequestFileDownloadAsync(Guid roomId, FileMetadataMessage fileMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отменить загрузку файла
    /// </summary>
    Task CancelFileDownloadAsync(Guid roomId, FileMetadataMessage fileMessage, CancellationToken cancellationToken = default);
}
