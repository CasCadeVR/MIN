using MIN.Handlers.Contracts.Models;
using MIN.Messaging.Contracts;

namespace MIN.Handlers.Contracts.Dispatcher;

/// <summary>
/// Диспетчер сообщений, отвечающий за маршрутизацию входящих сообщений к обработчикам
/// </summary>
public interface IMessageDispatcher
{
    /// <summary>
    /// Асинхронно отправляет сообщение в диспетчер для обработки
    /// </summary>
    Task DispatchAsync(IMessage message, MessageContext context);
}
