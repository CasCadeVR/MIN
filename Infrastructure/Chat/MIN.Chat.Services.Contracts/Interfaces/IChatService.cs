using MIN.FileTransfer.Messaging;

namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для работы с чатом
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Отправить текстовое сообщение
    /// </summary>
    Task SendMessageAsync(Guid roomId, string content, Guid? recipientId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить файл
    /// </summary>
    Task SendFileAsync(Guid roomId, string fileName, string filePath, Guid? recipientId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запросить загрузку файла
    /// </summary>
    Task RequestFileDownloadAsync(Guid roomId, FileMetadataMessage fileMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отменить загрузку файла
    /// </summary>
    Task CancelFileDownloadAsync(Guid roomId, FileMetadataMessage fileMessage, CancellationToken cancellationToken = default);
}
