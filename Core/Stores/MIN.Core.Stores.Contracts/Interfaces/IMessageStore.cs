using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Stores.Contracts.Interfaces;

/// <summary>
/// Хранилище сообщений для комнаты
/// </summary>
public interface IMessageStore
{
    /// <summary>
    /// Добавить сообщение
    /// </summary>
    void AddMessage(IMessage message);

    /// <summary>
    /// Получить историю сообщений
    /// </summary>
    IEnumerable<IMessage> GetHistory(int? page = 1, int? pageSize = 100);

    /// <summary>
    /// Получить последнее сообщение
    /// </summary>
    IMessage GetLastMessage();

    /// <summary>
    /// Очистить сообщения из комнаты
    /// </summary>
    void ClearMessages();
}
