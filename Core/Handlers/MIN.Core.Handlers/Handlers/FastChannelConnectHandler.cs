using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Exceptions;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.FastChannelConnect;
using MIN.Core.Messaging.Stateless.RoomRelated.Join;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class FastChannelConnectHandler : BaseHandler
{
    private readonly ITransport transport;
    private readonly IRoomConnectionRegistry registry;

    public FastChannelConnectHandler(ITransport transport,
        IRoomConnectionRegistry registry,
        ILoggerProvider logger) : base(logger)
    {
        this.transport = transport;
        this.registry = registry;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes
        => [MessageTypeTag.FastChannelConnectRequest, MessageTypeTag.FastChannelConnectResponse];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case FastChannelConnectRequestMessage requestMessage:
                var serverConnectionId = registry.GetServerConnectionIdByRoomId(context.RoomContext.RoomId);
                var udpEndpoint = transport.GetEndpoints(serverConnectionId)
                    .FirstOrDefault(ep => ep.Type == TransportType.Udp && ep.Origin == requestMessage.AddressOrigin);

                if (udpEndpoint == null)
                {
                    // остаёмся на TCP
                    return HandlerResult.Success();
                }

                return HandlerResult.WithResponse(new FastChannelConnectResponseMessage { FastChannelEndpoint = udpEndpoint });

            case FastChannelConnectResponseMessage response:
                await transport.ConnectAsync(response.FastChannelEndpoint, context.ConnectionId, context.CancellationToken);
                return HandlerResult.WithResponse(new RoomJoinRequestMessage());

            default:
                throw new HandlerTypeMismatch(this, message);
        }
    }
}
