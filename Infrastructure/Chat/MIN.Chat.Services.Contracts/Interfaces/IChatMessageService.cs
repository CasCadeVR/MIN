namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для функциональностей с сообщениями в чате
/// </summary>
public interface IChatMessageService
{
    /// <summary>
    /// Удалить сообщение
    /// </summary>
    Task DeleteMessageAsync(Guid roomId, Guid messageId, CancellationToken cancellationToken = default);
}
