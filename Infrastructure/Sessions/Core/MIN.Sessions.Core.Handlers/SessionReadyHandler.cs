using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionReadyHandler : BaseHandler
{
    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionReadyHandler"/>
    /// </summary>
    public SessionReadyHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.SessionReady];

    protected override Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var sessionReadyMessage = (SessionReadyMessage)message;
        context.RoomContext.Messages.AddMessage(sessionReadyMessage);

        return Task.FromResult(HandlerResult.WithEvent(new SessionReadyMessageReceivedEvent()
        {
            Message = sessionReadyMessage,
            RoomId = context.RoomContext.RoomId,
        }));
    }
}
