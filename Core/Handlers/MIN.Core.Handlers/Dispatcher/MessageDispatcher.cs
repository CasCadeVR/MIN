using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Dispatcher
{
    /// <inheritdoc cref="IMessageDispatcher"/>
    public sealed class MessageDispatcher : IMessageDispatcher
    {
        private readonly IEnumerable<IMessageHandler> handlers;
        private readonly IMessageSender messageSender;
        private readonly IRoomHoster roomHoster;
        private readonly IEventBus eventBus;
        private readonly IParticipantConnectionRegistry participantConnectionRegistry;
        private readonly ILoggerProvider logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageDispatcher"/>
        /// </summary>
        public MessageDispatcher(IEnumerable<IMessageHandler> handlers,
            IMessageSender messageSender,
            IRoomHoster roomHoster,
            IEventBus eventBus,
            IParticipantConnectionRegistry participantConnectionRegistry,
            ILoggerProvider logger)
        {
            this.handlers = handlers;
            this.messageSender = messageSender;
            this.roomHoster = roomHoster;
            this.eventBus = eventBus;
            this.participantConnectionRegistry = participantConnectionRegistry;
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

                    if (!result.IsSuccess)
                    {
                        logger.Log($"Обработчик {handler.GetType().Name} провалился: {result.ErrorMessage}");
                        await PublishErrorEvent(result.ErrorMessage!, result.StopPropagation, context);
                        break;
                    }

                    if (result.StopPropagation)
                    {
                        break;
                    }

                    if (message.IsPublic && roomHoster.IsHosting(context.RoomId))
                    {
                        var senderConnectionId = participantConnectionRegistry.GetConnectionIdFromParticipantId(context.RoomId, context.SenderId);
                        await messageSender.BroadcastAsync(message, context.RoomId, context.SenderId, [senderConnectionId], context.CancellationToken);
                    }

                    if (result.Response != null)
                    {
                        await messageSender.SendAsync(result.Response, context.RoomId, context.SenderId, context.ConnectionId, context.CancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log($"Handler {handler.GetType().Name} threw exception: {ex.Message}");
                    await PublishErrorEvent(ex.Message, needToDisconnect: true, context);
                }
            }
        }

        private async Task PublishErrorEvent(string message, bool needToDisconnect, MessageContext context)
        {
            await eventBus.PublishAsync(new ErrorOccurredEvent()
            {
                ErrorMessage = message,
                NeedToDisconnect = needToDisconnect,
                RoomId = context.RoomId
            }, context.CancellationToken);
        }
    }
}
