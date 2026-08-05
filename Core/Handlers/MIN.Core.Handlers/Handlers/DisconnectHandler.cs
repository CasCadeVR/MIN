using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.Disconnect;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Handlers.Handlers;

internal sealed class DisconnectHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly IMessageSender messageSender;
    private readonly IRoomStore roomStore;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    public DisconnectHandler(IEventBus eventBus,
        IMessageSender messageSender,
        IRoomStore roomStore,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.messageSender = messageSender;
        this.roomStore = roomStore;
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
                await messageSender.SendAsync(new DisconnectAckMessage()
                {
                    Reason = reason,
                    SenderId = identityService.SelfParticipant.Id,
                }, context.RoomContext.RoomId, context.ConnectionId, context.CancellationToken);

                var roomName = roomStore.GetRoom(context.RoomContext.RoomId).Name;
                var uiToShow = "Хост кикнул тебя" + (roomName != null ? $" из комнаты {roomName}" : string.Empty) + (reason != string.Empty ? $": {reason}" : string.Empty);
                return HandlerResult.Failure(uiToShow, stopPropagation: true, critical: true);

            case DisconnectAckMessage disconnectAckMessage:
                await eventBus.PublishAsync(new DisconnectAckReceived()
                {
                    ParticipantId = message.SenderId,
                    RoomId = context.RoomContext.RoomId,
                }, context.CancellationToken);
                return HandlerResult.Success();

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(DisconnectHandler)} - {message.GetType()}");
        }
    }
}
