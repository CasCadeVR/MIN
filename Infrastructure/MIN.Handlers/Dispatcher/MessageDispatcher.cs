using MIN.Handlers.Contracts;
using MIN.Handlers.Contracts.Dispatcher;
using MIN.Handlers.Contracts.Models;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Services.Contracts.Interfaces;

namespace MIN.Handlers.Dispatcher
{
    /// <summary>
    /// Реализация диспетчера сообщений
    /// </summary>
    public sealed class MessageDispatcher : IMessageDispatcher
    {
        private readonly IEnumerable<IMessageHandler> handlers;
        private readonly ILoggerProvider logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageDispatcher"/>
        /// </summary>
        public MessageDispatcher(IEnumerable<IMessageHandler> handlers, ILoggerProvider logger)
        {
            this.handlers = handlers;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task DispatchAsync(IMessage message, MessageContext context)
        {
            var applicableHandlers = handlers
                .Where(h => h.HandledTypes.Contains(message.TypeTag))
                .OrderBy(h => h.Priority)
                .ToList();

            if (applicableHandlers.Count == 0)
            {
                logger.Log($"No handler registered for message type {message.TypeTag}");
                return;
            }

            foreach (var handler in applicableHandlers)
            {
                try
                {
                    var result = await handler.HandleAsync(message, context);
                    if (result.StopPropagation)
                    {
                        break;
                    }

                    if (!result.IsSuccess)
                    {
                        logger.Log($"Handler {handler.GetType().Name} failed: {result.ErrorMessage}");
                    }
                    else if (result.Response != null && context.Transport != null)
                    {
                        // Отправляем ответное сообщение отправителю
                        var responseData = context.Transport.Serialize(result.Response);
                        await context.Transport.SendAsync(responseData, context.Sender, context.CancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log($"Handler {handler.GetType().Name} threw exception: {ex.Message}");
                }
            }
        }
    }
}
