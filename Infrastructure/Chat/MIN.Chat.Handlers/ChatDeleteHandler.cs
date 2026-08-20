using MIN.Chat.Events;
using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Moderation;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Chat.Handlers;

internal sealed class ChatDeleteHandler : IMessageHandler
{
    private readonly static List<MessageTypeTag> allowedMessagesToDelete = [MessageTypeTag.ChatTextMessage, MessageTypeTag.FileMetadata];

    private readonly IEventBus eventBus;
    private readonly INetworkErrorHandler errorHandler;
    private readonly ILoggerProvider logger;


    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatDeleteHandler"/>
    /// </summary>
    public ChatDeleteHandler(IEventBus eventBus,
        INetworkErrorHandler errorHandler,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.errorHandler = errorHandler;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.MessageDelete];

    int IMessageHandler.Priority => 12;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not ChatDeleteMessage chatDeleteMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(ChatTextHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChatTextHandler)} - {message.GetType()}");
        }

        var existingMessage = context.RoomContext.Messages.GetMessageById(chatDeleteMessage.MessageIdToDelete);
        if (existingMessage == null)
        {
            logger.Log("Поступило сообщение на удаление, но его не нашлось в памяти", LogLevel.Warning);

            if (context.Role == Role.Host)
            {
                await errorHandler.SendErrorAsync("Сообщение, которое вы хотели удалить, не найдено", message.SenderId, context.RoomContext.RoomId);
            }

            return HandlerResult.Success();
        }

        if (context.Role == Role.Host)
        {
            if (existingMessage.SenderId != message.SenderId)
            {
                await errorHandler.SendErrorAsync("Сообщение, которое вы хотели удалить, было отправлено не вами", message.SenderId, context.RoomContext.RoomId);
                return HandlerResult.Success(stopPropagation: false);
            }

            if (!allowedMessagesToDelete.Contains(existingMessage.TypeTag))
            {
                await errorHandler.SendErrorAsync("Сообщение, которое вы хотели удалить, не подлежит удалению", message.SenderId, context.RoomContext.RoomId);
                return HandlerResult.Success();
            }
        }

        context.RoomContext.Messages.RemoveMessage(chatDeleteMessage.MessageIdToDelete);

        await eventBus.PublishAsync(new ChatMessageDeletedEvent()
        {
            MessageId = chatDeleteMessage.MessageIdToDelete,
            RoomId = context.RoomContext.RoomId,
        });

        return HandlerResult.Success();
    }
}
