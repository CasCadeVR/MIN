using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ErrorHandler : IMessageHandler
{
    private readonly ILoggerProvider logger;

    public ErrorHandler(ILoggerProvider logger)
    {
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.Error];

    int IMessageHandler.Priority => 3;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not ErrorMessage errorMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(ErrorHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ErrorHandler)} - {message.GetType()}");
        }

        logger.Log($"Ошибка, полученная от получателя {context.RoomContext.Participants.GetParticipantById(message.SenderId).Name}: {errorMessage.Message}");
        return HandlerResult.Failure(errorMessage.Message);
    }
}
