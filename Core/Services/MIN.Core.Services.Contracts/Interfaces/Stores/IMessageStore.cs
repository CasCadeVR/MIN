using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Services.Contracts.Interfaces.Stores;

/// <summary>
/// Хранилище сообщений для комнаты
/// </summary>
public interface IMessageStore
{
    /// <summary>
    /// Добавить сообщение
    /// </summary>
    void AddMessage(Guid roomId, IMessage message);

    /// <summary>
    /// Получить историю сообщений
    /// </summary>
    IEnumerable<IMessage> GetHistory(Guid roomId, int? page = 1, int? pageSize = 100);

    /// <summary>
    /// Очистить сообщения из комнаты
    /// </summary>
    void ClearMessages(Guid roomId);
}
