namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для функциональностей с сообщениями в чате
/// </summary>
public interface IChatMessageService
{
    /// <summary>
    /// Редактировать сообщение, содержащий текстовый контент
    /// </summary>
    Task EditTextMessageAsync(Guid roomId, Guid messageId, string newContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить сообщение
    /// </summary>
    Task DeleteMessageAsync(Guid roomId, Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Очистить историю сообщений по текущее время
    /// </summary>
    Task ClearMessageHistoryAsync(Guid roomId, CancellationToken cancellationToken = default);
}
