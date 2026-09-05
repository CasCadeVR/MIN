using MIN.Chat.Messaging;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Handlers;

internal sealed class ChatEditHandler : BaseHandler
{
    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatEditHandler"/>
    /// </summary>
    public ChatEditHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.MessageEdit];

    protected override Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var chatEditMessage = (ChatEditMessage)message;

        var existingMessage = context.RoomContext.Messages.GetMessageById(chatEditMessage.MessageIdToEdit);
        if (existingMessage == null)
        {
            LogWarning("Поступило сообщение на редактирование, но его не нашлось в памяти");

            if (context.Role == Role.Host)
            {
                return Task.FromResult(HandlerResult.WithErrorHandled("Сообщение, которое вы хотели отредактировать, не найдено"));
            }

            return Task.FromResult(HandlerResult.Success());
        }

        if (context.Role == Role.Host)
        {
            if (existingMessage.SenderId != message.SenderId)
            {
                return Task.FromResult(HandlerResult.WithErrorHandled("Сообщение, которое вы хотели отредактировать, было отправлено не вами"));
            }

            if (existingMessage is not IContentEditable)
            {
                return Task.FromResult(HandlerResult.WithErrorHandled("Сообщение, которое вы хотели отредактировать, не может быть отредактировано"));
            }
        }

        if (existingMessage is IContentEditable contentEditable)
        {
            contentEditable.Content = chatEditMessage.NewContent;
            contentEditable.IsEdited = true;
            contentEditable.EditedAt = DateTime.Now;

            context.RoomContext.Messages.UpdateMessage(chatEditMessage.MessageIdToEdit, existingMessage);

            var replyables = context.RoomContext.Messages.GetHistory().OfType<IReplyable>();
            foreach (var replyable in replyables)
            {
                if (replyable.ReplyToMessageId == chatEditMessage.MessageIdToEdit)
                {
                    replyable.ReplyToMessageDescription = (contentEditable as IDescribable)?.GetDescription();
                }
            }

            return Task.FromResult(HandlerResult.WithEvent(new MessageEditedEvent() { MessageId = chatEditMessage.MessageIdToEdit, Message = contentEditable, RoomId = context.RoomContext.RoomId }));
        }

        return Task.FromResult(HandlerResult.Success());
    }
}
