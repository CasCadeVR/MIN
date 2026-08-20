using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Constants;

namespace MIN.Core.Stores.Contracts.Interfaces;

/// <summary>
/// Хранилище сообщений для комнаты
/// </summary>
public interface IMessageStore
{
    /// <summary>
    /// Добавить сообщение
    /// </summary>
    void AddMessage(IMessage message, bool appendOnStart = false);

    /// <summary>
    /// Обновить сообщение
    /// </summary>
    void UpdateMessage(Guid id, IMessage message);

    /// <summary>
    /// Получить сообщение по Id
    /// </summary>
    IMessage? GetMessageById(Guid id);

    /// <summary>
    /// Получить количество сохранённых сообщений
    /// </summary>
    int GetMessageCount();

    /// <summary>
    /// Получить историю последних сообщений
    /// </summary>
    IEnumerable<IMessage> GetRecentHistory(int page = 1, int pageSize = StoreConstants.MessagesPageSize);

    /// <summary>
    /// Получить историю сообщений
    /// </summary>
    IEnumerable<IMessage> GetHistory(int? page = null, int? pageSize = null);

    /// <summary>
    /// Получить последнее сообщение
    /// </summary>
    IMessage GetLastMessage();

    /// <summary>
    /// Удалить сообщение по Id
    /// </summary>
    void RemoveMessage(Guid id);

    /// <summary>
    /// Очистить сообщения из комнаты
    /// </summary>
    void ClearMessages();
}
