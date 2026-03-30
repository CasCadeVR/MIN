using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts.Models;

namespace MIN.Core.Handlers.Contracts.Dispatcher;

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
