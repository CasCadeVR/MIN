using MIN.Chat.Events;
using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Handlers;

internal sealed class ChatTextHandler : BaseHandler
{
    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatTextHandler"/>
    /// </summary>
    public ChatTextHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.ChatTextMessage];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var chatTextMessage = (ChatTextMessage)message;

        if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
        }

        context.RoomContext.Messages.AddMessage(chatTextMessage);

        if (message.SenderId == context.SelfId || message.RecipientId == context.SelfId || message.IsPublic)
        {
            return HandlerResult.WithEvent(new ChatTextMessageReceivedEvent()
            {
                Message = chatTextMessage,
                RoomId = context.RoomContext.RoomId,
                Sender = sender!.ToParticipantInfo(),
            });
        }

        return HandlerResult.Success();
    }
}
