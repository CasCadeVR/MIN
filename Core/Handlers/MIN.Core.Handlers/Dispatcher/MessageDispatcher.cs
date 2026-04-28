using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Handlers.Dispatcher;

/// <inheritdoc cref="IMessageDispatcher"/>
public sealed class MessageDispatcher : IMessageDispatcher
{
    private readonly IEnumerable<IMessageHandler> handlers;
    private readonly IMessageSender messageSender;
    private readonly IIdentityService identityService;
    private readonly IRoomHoster roomHoster;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MessageDispatcher"/>
    /// </summary>
    public MessageDispatcher(IEnumerable<IMessageHandler> handlers,
        IMessageSender messageSender,
        IIdentityService identityService,
        IRoomHoster roomHoster,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.handlers = handlers;
        this.messageSender = messageSender;
        this.identityService = identityService;
        this.roomHoster = roomHoster;
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

                if (roomHoster.IsHosting(context.RoomContext.RoomId))
                {
                    await HandleServerMessageRouting(message, context);
                }

                if (result.Response != null)
                {
                    result.Response.SenderId = identityService.SelfPartcipant.Id;
                    await messageSender.SendAsync(result.Response, context.RoomContext.RoomId, context.ConnectionId, context.CancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Handler {handler.GetType().Name} threw exception: {ex.Message}");
                await PublishErrorEvent(ex.Message, needToDisconnect: true, context);
            }
        }
    }

    private async Task HandleServerMessageRouting(IMessage message, MessageContext context)
    {
        if (message.IsPublic)
        {
            var senderConnectionId = context.RoomContext.Connections.GetConnectionIdFromParticipantId(message.SenderId);
            await messageSender.BroadcastAsync(message, context.RoomContext.RoomId, [senderConnectionId], context.CancellationToken);
        }
        else if (message.RecipientId != null)
        {
            if (!context.RoomContext.Connections.TryGetConnectionIdFromParticipantId(message.RecipientId ?? Guid.Empty, out var recipientConnectionId))
            {
                logger.Log($"Не удалось найти участника с id {message.RecipientId} во время маршрутизации приватного сообщения", LogLevel.Error);
                return;
            }

            if (recipientConnectionId != CoreRegistryConstants.LocalConnectionId)
            {
                await messageSender.SendAsync(message, context.RoomContext.RoomId, recipientConnectionId, context.CancellationToken);
            }
        }
    }

    private async Task PublishErrorEvent(string message, bool needToDisconnect, MessageContext context)
    {
        await eventBus.PublishAsync(new ErrorOccurredEvent()
        {
            ErrorMessage = message,
            NeedToDisconnect = needToDisconnect,
            RoomId = context.RoomContext.RoomId
        }, context.CancellationToken);
    }
}
