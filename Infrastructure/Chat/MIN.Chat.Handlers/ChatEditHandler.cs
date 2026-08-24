using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Moderation;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Chat.Handlers;

internal sealed class ChatEditHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly INetworkErrorHandler errorHandler;
    private readonly ILoggerProvider logger;


    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatEditHandler"/>
    /// </summary>
    public ChatEditHandler(IEventBus eventBus,
        INetworkErrorHandler errorHandler,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.errorHandler = errorHandler;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.MessageEdit];

    int IMessageHandler.Priority => 16;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not ChatEditMessage chatEditMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(ChatTextHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChatTextHandler)} - {message.GetType()}");
        }

        var existingMessage = context.RoomContext.Messages.GetMessageById(chatEditMessage.MessageIdToEdit);
        if (existingMessage == null)
        {
            logger.Log("Поступило сообщение на редактирование, но его не нашлось в памяти", LogLevel.Warning);

            if (context.Role == Role.Host)
            {
                await errorHandler.SendErrorAsync("Сообщение, которое вы хотели отредактировать, не найдено", message.SenderId, context.RoomContext.RoomId);
            }

            return HandlerResult.Success();
        }

        if (context.Role == Role.Host)
        {
            if (existingMessage.SenderId != message.SenderId)
            {
                await errorHandler.SendErrorAsync("Сообщение, которое вы хотели отредактировать, было отправлено не вами", message.SenderId, context.RoomContext.RoomId);
                return HandlerResult.Success(stopPropagation: false);
            }

            if (existingMessage is not IContentEditable)
            {
                await errorHandler.SendErrorAsync("Сообщение, которое вы хотели отредактировать, не может быть отредактировано", message.SenderId, context.RoomContext.RoomId);
                return HandlerResult.Success();
            }
        }

        if (existingMessage is IContentEditable contentEditable)
        {
            contentEditable.Content = chatEditMessage.NewContent;
            contentEditable.IsEdited = true;
            contentEditable.EditedAt = DateTime.Now;

            // TODO: Надо бы что-то предпринять, ибо это по сути вообще ничего не делает
            context.RoomContext.Messages.UpdateMessage(chatEditMessage.MessageIdToEdit, existingMessage);

            await eventBus.PublishAsync(new MessageEditedEvent()
            {
                MessageId = chatEditMessage.MessageIdToEdit,
                Message = contentEditable,
                RoomId = context.RoomContext.RoomId,
            });
        }

        return HandlerResult.Success();
    }
}
