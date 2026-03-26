using MIN.Events.Contracts;
using MIN.Events.Events;
using MIN.Handlers.Contracts;
using MIN.Handlers.Contracts.Dispatcher;
using MIN.Handlers.Contracts.Models;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Services.Contracts.Interfaces;

namespace MIN.Handlers.Dispatcher
{
    /// <inheritdoc cref="IMessageDispatcher"/>
    public sealed class MessageDispatcher : IMessageDispatcher
    {
        private readonly IEnumerable<IMessageHandler> handlers;
        private readonly IMessageService messageService;
        private readonly IEventBus eventBus;
        private readonly ILoggerProvider logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageDispatcher"/>
        /// </summary>
        public MessageDispatcher(IEnumerable<IMessageHandler> handlers,
            IMessageService messageService,
            IEventBus eventBus,
            ILoggerProvider logger)
        {
            this.handlers = handlers;
            this.messageService = messageService;
            this.eventBus = eventBus;
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
                        await PublishErrorEvent(result.ErrorMessage!, context);
                    }
                    else if (result.Response != null)
                    {
                        await messageService.SendAsync(result.Response, context.RoomId, context.ConnectionId, context.CancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log($"Handler {handler.GetType().Name} threw exception: {ex.Message}");
                    await PublishErrorEvent(ex.Message, context);
                }
            }
        }

        private async Task PublishErrorEvent(string message, MessageContext context)
        {
            await eventBus.PublishAsync(new ErrorOccurredEvent()
            {
                ErrorMessage = message,
                RoomId = context.RoomId
            }, context.CancellationToken);
        }
    }
}
