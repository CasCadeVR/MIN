using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ErrorHandler : IMessageHandler
{
    private readonly ILoggerProvider logger;
    private readonly IIdentityService identityService;

    public ErrorHandler(ILoggerProvider logger,
        IIdentityService identityService)
    {
        this.logger = logger;
        this.identityService = identityService;
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

        if (message.RecipientId != null && identityService.SelfParticipant.Id != message.RecipientId)
        {
            return HandlerResult.Success();
        }

        logger.Log($"Ошибка, полученная от получателя {context.RoomContext.Participants.GetParticipantById(message.SenderId).Name}: {errorMessage.Message}");
        return HandlerResult.Failure(errorMessage.Message);
    }
}
