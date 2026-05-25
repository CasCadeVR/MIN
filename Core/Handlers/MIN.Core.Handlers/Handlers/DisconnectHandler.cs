using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.Disconnect;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Handlers.Handlers;

internal sealed class DisconnectHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="DisconnectHandler"/>
    /// </summary>
    public DisconnectHandler(IEventBus eventBus,
        IMessageRouter messageRouter,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.Disconnect, MessageTypeTag.DisconnectAck];

    int IMessageHandler.Priority => 0;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case DisconnectMessage disconnectMessage:
                var reason = disconnectMessage.Reason;
                logger.Log($"Сервер нарошно отключил меня: {reason}", LogLevel.Error);
                await messageRouter.RouteAsync(new DisconnectAckMessage()
                {
                    Reason = reason,
                }, context.RoomContext.RoomId, identityService.SelfParticipant.Id, context.CancellationToken);
                return HandlerResult.Failure(reason, stopPropagation: true, critical: true);

            case DisconnectAckMessage disconnectAckMessage:
                await eventBus.PublishAsync(new DisconnectAckReceived()
                {
                    ConnectionId = context.ConnectionId,
                    RoomId = context.RoomContext.RoomId,
                }, context.CancellationToken);
                return HandlerResult.Success();

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantJoinHandler)} - {message.GetType()}");
        }
    }
}
