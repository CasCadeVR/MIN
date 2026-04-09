using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;

namespace MIN.Core.Handlers.Dispatcher
{
    /// <inheritdoc cref="IMessageDispatcher"/>
    public sealed class MessageDispatcher : IMessageDispatcher
    {
        private readonly IEnumerable<IMessageHandler> handlers;
        private readonly IMessageSender messageSender;
        private readonly IMessageRouter messageRouter;
        private readonly IEventBus eventBus;
        private readonly ILoggerProvider logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageDispatcher"/>
        /// </summary>
        public MessageDispatcher(IEnumerable<IMessageHandler> handlers,
            IMessageSender messageSender,
            IMessageRouter messageRouter,
            IEventBus eventBus,
            ILoggerProvider logger)
        {
            this.handlers = handlers;
            this.messageRouter = messageRouter;
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
                throw new NotImplementedException($"Не зарегистрирован обработчик для {message.TypeTag}");
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
                    else if (message.IsPublic)
                    {
                        await messageRouter.RouteAsync(message, context.RoomId, context.Sender.Id, context.CancellationToken);
                    }
                    else if (result.Response != null)
                    {
                        await messageSender.SendAsync(result.Response, context.RoomId, context.Sender.Id, context.ConnectionId, context.CancellationToken);
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
