using MIN.Core.Entities.Contracts.Models;

namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для работы с чатом
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Отправить текстовое сообщение
    /// </summary>
    Task SendMessageAsync(Guid roomId, string content, ParticipantInfo sender, Guid? recipientId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить файл
    /// </summary>
    Task SendFileAsync(Guid roomId, string fileName, string filePath, ParticipantInfo sender, Guid? recipientId = null, CancellationToken cancellationToken = default);
}
