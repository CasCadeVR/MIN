using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Chat.Messaging;
using MIN.Chat.Events;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Events.Contracts;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ChatTextMessage"/>
/// </summary>
internal sealed class ChatTextHandler : IMessageHandler, IChatHandlerAnchor
{
    private readonly IIdentityService identityService;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatTextHandler"/>
    /// </summary>
    public ChatTextHandler(IIdentityService identityService,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.identityService = identityService;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.ChatTextMessage];

    int IMessageHandler.Priority => 10;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not ChatTextMessage chatTextMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(ChatTextHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChatTextHandler)} - {message.GetType()}");
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false);
        }

        context.RoomContext.Messages.AddMessage(chatTextMessage);
        var selfId = identityService.SelfParticipant.Id;

        if (message.SenderId == selfId || message.RecipientId == selfId || message.IsPublic)
        {
            await eventBus.PublishAsync(new ChatTextMessageReceivedEvent()
            {
                Message = chatTextMessage,
                RoomId = context.RoomContext.RoomId,
                Sender = sender!,
            });
        }

        return HandlerResult.Success();
    }
}
