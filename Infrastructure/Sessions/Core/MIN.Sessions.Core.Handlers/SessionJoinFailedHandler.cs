using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionJoinFailedHandler : IMessageHandler
{
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionJoinFailedHandler"/>
    /// </summary>
    public SessionJoinFailedHandler(ILoggerProvider logger)
    {
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionJoinFailed];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionJoinFailedMessage sessionJoinFailedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionJoinFailedHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionJoinFailedHandler)} - {message.GetType()}");
        }

        logger.Log($"Не удалось войти в сессию: {sessionJoinFailedMessage.ErrorMessage}");
        return HandlerResult.Failure($"Не удалось войти в сессию: {sessionJoinFailedMessage.ErrorMessage}");
    }
}
