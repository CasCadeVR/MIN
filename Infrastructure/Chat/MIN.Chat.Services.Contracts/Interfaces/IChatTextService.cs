namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для работы с текстовыми сообщениями в чате
/// </summary>
public interface IChatTextService
{
    /// <summary>
    /// Отправить текстовое сообщение
    /// </summary>
    Task SendMessageAsync(Guid roomId, string content, Guid? recipientId = null, CancellationToken cancellationToken = default);
}
