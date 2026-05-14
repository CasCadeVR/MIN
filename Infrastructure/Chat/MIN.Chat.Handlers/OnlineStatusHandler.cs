using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Chat.Messaging;
using MIN.Chat.Events;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Events.Contracts;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Handlers;

internal sealed class OnlineStatusHandler : IMessageHandler, IChatHandlerAnchor
{
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatTextHandler"/>
    /// </summary>
    public OnlineStatusHandler(IEventBus eventBus, ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.OnlineStatusChanged];

    int IMessageHandler.Priority => 8;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not OnlineStatusChangedMessage onlineStatusChangedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(OnlineStatusHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(OnlineStatusHandler)} - {message.GetType()}");
        }

        await eventBus.PublishAsync(new OnlineStatusChangedEvent()
        {
            RoomId = context.RoomContext.RoomId,
            Status = onlineStatusChangedMessage.Status,
            SenderId = message.SenderId,
        });

        return HandlerResult.Success();
    }
}
