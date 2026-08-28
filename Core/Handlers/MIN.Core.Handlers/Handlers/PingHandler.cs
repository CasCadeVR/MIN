using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Exceptions;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.Ping;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class PingHandler : BaseHandler
{
    private readonly IEventBus eventBus;

    public PingHandler(IEventBus eventBus, ILoggerProvider logger) : base(logger)
    {
        this.eventBus = eventBus;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes
        => [MessageTypeTag.Ping, MessageTypeTag.Pong];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case PingMessage _:
                await eventBus.PublishAsync(GetNotificatonPingService(context), context.CancellationToken);
                return HandlerResult.WithResponse(new PongMessage());

            case PongMessage _:
                return HandlerResult.WithEvent(GetNotificatonPingService(context));

            default:
                throw new HandlerTypeMismatch(this, message);
        }
    }

    private static PingPongReceivedEvent GetNotificatonPingService(MessageContext context)
        => new()
        {
            Role = context.Role,
            ConnectionId = context.ConnectionId,
            RoomId = context.RoomContext.RoomId,
        };
}
