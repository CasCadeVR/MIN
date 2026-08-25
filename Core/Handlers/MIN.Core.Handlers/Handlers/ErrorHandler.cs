using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ErrorHandler : BaseHandler
{
    public ErrorHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.Error];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var errorMessage = (ErrorMessage)message;

        if (message.RecipientId != null && context.SelfId != message.RecipientId && context.Role == Role.Host)
        {
            LogError($"Я, как хост, отправляю {context.RoomContext.Participants.GetParticipantById(message.SenderId).Name} ошибку: {errorMessage.Message}");
            return HandlerResult.Success();
        }

        LogError($"Ошибка, полученная от получателя {context.RoomContext.Participants.GetParticipantById(message.SenderId).Name}: {errorMessage.Message}");
        return HandlerResult.Failure(errorMessage.Message);
    }
}
