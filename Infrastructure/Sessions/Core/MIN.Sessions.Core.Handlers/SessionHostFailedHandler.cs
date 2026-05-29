using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionHostFailedHandler : IMessageHandler
{
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionReadyHandler"/>
    /// </summary>
    public SessionHostFailedHandler(ILoggerProvider logger)
    {
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionHostFailed];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionHostFailedMessage sessionHostFailedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionHostFailedHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionHostFailedHandler)} - {message.GetType()}");
        }

        logger.Log($"Хост не смог сделать сессию: {sessionHostFailedMessage.ErrorMessage}");
        return HandlerResult.Failure($"Хост не смог сделать сессию: {sessionHostFailedMessage.ErrorMessage}");
    }
}
