using MIN.Events.Contracts;
using MIN.Events.Events;
using MIN.Handlers.Contracts;
using MIN.Handlers.Contracts.Models;
using MIN.Messaging.Contracts;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Messaging.RoomRelated.ParticipantRelated;

namespace MIN.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ChatTextMessage"/>
/// </summary>
internal sealed class ChatTextHandler : IMessageHandler, IHandlerAnchor
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

    int IMessageHandler.Priority => 0;

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
