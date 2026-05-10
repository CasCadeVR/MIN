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
    /// Получить количество сохранённых сообщений
    /// </summary>
    int GetMessageCount();

    /// <summary>
    /// Получить историю последних сообщений
    /// </summary>
    IEnumerable<IMessage> GetRecentHistory(int page = 1, int pageSize = 25);

    /// <summary>
    /// Получить историю сообщений
    /// </summary>
    IEnumerable<IMessage> GetHistory(int? page = null, int? pageSize = null);

    /// <summary>
    /// Получить последнее сообщение
    /// </summary>
    IMessage GetLastMessage();

    /// <summary>
    /// Очистить сообщения из комнаты
    /// </summary>
    void ClearMessages();
}
