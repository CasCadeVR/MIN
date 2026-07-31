using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.Ping;
using MIN.Core.Services.Contracts.Interfaces.Messaging;

namespace MIN.Core.Handlers.Handlers;

internal sealed class PingHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly IMessageSender messageSender;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="PingHandler"/>
    /// </summary>
    public PingHandler(IEventBus eventBus, IMessageSender messageSender)
    {
        this.eventBus = eventBus;
        this.messageSender = messageSender;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.Ping, MessageTypeTag.Pong];

    int IMessageHandler.Priority => 0;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case PingMessage _:
                await messageSender.SendAsync(new PongMessage(), context.RoomContext.RoomId, context.ConnectionId, context.CancellationToken);
                await NotifyPingService(context);
                return HandlerResult.Success();

            case PongMessage _:
                await NotifyPingService(context);
                return HandlerResult.Success();

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(PingHandler)} - {message.GetType()}");
        }
    }

    private async Task NotifyPingService(MessageContext context)
    {
        await eventBus.PublishAsync(new PingPongReceivedEvent()
        {
            Role = context.Role,
            ConnectionId = context.ConnectionId,
            RoomId = context.RoomContext.RoomId,
        }, context.CancellationToken);
    }
}
