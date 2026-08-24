using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Handlers;

internal sealed class ChatDeleteHandler : BaseHandler
{
    private readonly static List<MessageTypeTag> allowedMessagesToDelete = [MessageTypeTag.ChatTextMessage, MessageTypeTag.FileMetadata];

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatDeleteHandler"/>
    /// </summary>
    public ChatDeleteHandler(ILoggerProvider logger) : base(logger) { }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.MessageDelete];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var chatDeleteMessage = (ChatDeleteMessage)message;

        var existingMessage = context.RoomContext.Messages.GetMessageById(chatDeleteMessage.MessageIdToDelete);

        if (existingMessage == null)
        {
            LogWarning("Поступило сообщение на удаление, но его не нашлось в памяти");

            if (context.Role == Role.Host)
            {
                return HandlerResult.WithErrorHandled("Сообщение, которое вы хотели удалить, не найдено");
            }

            return HandlerResult.Success();
        }

        if (context.Role == Role.Host)
        {
            if (existingMessage.SenderId != message.SenderId)
            {
                return HandlerResult.WithErrorHandled("Сообщение, которое вы хотели удалить, было отправлено не вами");
            }

            if (!allowedMessagesToDelete.Contains(existingMessage.TypeTag))
            {
                return HandlerResult.WithErrorHandled("Сообщение, которое вы хотели удалить, не подлежит удалению");
            }
        }

        context.RoomContext.Messages.RemoveMessage(chatDeleteMessage.MessageIdToDelete);

        return HandlerResult.WithEvent(new MessageDeletedEvent()
        {
            MessageId = chatDeleteMessage.MessageIdToDelete,
            RoomId = context.RoomContext.RoomId,
        });
    }
}
