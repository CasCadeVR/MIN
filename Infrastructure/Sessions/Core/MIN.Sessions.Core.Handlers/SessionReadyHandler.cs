using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionReadyHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionReadyHandler"/>
    /// </summary>
    public SessionReadyHandler(IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionReady];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionReadyMessage sessionReadyMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionReadyHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionReadyHandler)} - {message.GetType()}");
        }

        context.RoomContext.Messages.AddMessage(sessionReadyMessage);

        await eventBus.PublishAsync(new SessionReadyMessageReceivedEvent()
        {
            Message = sessionReadyMessage,
            RoomId = context.RoomContext.RoomId,
        });

        return HandlerResult.Success();
    }
}
