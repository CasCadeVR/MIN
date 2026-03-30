using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Chat.Messaging;
using MIN.Chat.Events;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Events.Contracts;

namespace MIN.Chat.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ChatTextMessage"/>
/// </summary>
internal sealed class ChatTextHandler : IMessageHandler, IChatHandlerAnchor
{
    private readonly IEventBus eventBus;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="HandshakeHandler"/>
    /// </summary>
    public ChatTextHandler(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.ChatTextMessage];

    int IMessageHandler.Priority => 10;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is ChatTextMessage chatTextMessage)
        {
            await eventBus.PublishAsync(new ChatTextMessageReceivedEvent()
            {
                Message = chatTextMessage,
                RoomId = context.RoomId,
                Sender = context.Sender,
            });

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChatTextHandler)} - {message.GetType()}");
    }
}
