using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts.Models;

namespace MIN.Core.Handlers.Contracts
{
    /// <summary>
    /// Обработчик сообщений определённого типа
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Типы сообщений, которые обрабатывает данный обработчик
        /// </summary>
        IEnumerable<MessageTypeTag> HandledTypes { get; }

        /// <summary>
        /// Приоритет обработчика (меньшее значение – выше приоритет)
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Асинхронно обрабатывает сообщение
        /// </summary>
        Task<HandlerResult> HandleAsync(IMessage message, MessageContext context);
    }
}
