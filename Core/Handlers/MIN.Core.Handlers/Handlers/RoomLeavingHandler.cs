using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Exceptions;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.Leaving;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class RoomLeavingHandler : BaseHandler
{
    public RoomLeavingHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes
        => [MessageTypeTag.RoomLeave, MessageTypeTag.RoomLeaveAck];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var roomId = context.RoomContext.RoomId;

        switch (message)
        {
            case RoomLeaveMessage _:
                return HandlerResult.WithResponse(new RoomLeaveMessageAckMessage());

            case RoomLeaveMessageAckMessage _:
                return HandlerResult.Success();

            default:
                throw new HandlerTypeMismatch(this, message);
        }
    }
}
