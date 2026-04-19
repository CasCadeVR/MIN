using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Chat.Messaging;
using MIN.Chat.Events;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Events.Contracts;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ChatTextMessage"/>
/// </summary>
internal sealed class ChatTextHandler : IMessageHandler, IChatHandlerAnchor
{
    private readonly IMessageStore messageStore;
    private readonly IParticipantStore participantStore;
    private readonly IIdentityService identityService;
    private readonly IEventBus eventBus;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChatTextHandler"/>
    /// </summary>
    public ChatTextHandler(IMessageStore messageStore,
        IParticipantStore participantStore,
        IIdentityService identityService,
        IEventBus eventBus)
    {
        this.messageStore = messageStore;
        this.participantStore = participantStore;
        this.identityService = identityService;
        this.eventBus = eventBus;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.ChatTextMessage];

    int IMessageHandler.Priority => 10;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is ChatTextMessage chatTextMessage)
        {
            if (!participantStore.TryGetParticipantById(context.RoomId, message.SenderId, out var sender))
            {
                return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false);
            }

            messageStore.AddMessage(context.RoomId, chatTextMessage);
            var selfId = identityService.SelfPartcipant.Id;

            if (message.SenderId == selfId || message.RecipientId == selfId || message.IsPublic)
            {
                await eventBus.PublishAsync(new ChatTextMessageReceivedEvent()
                {
                    Message = chatTextMessage,
                    RoomId = context.RoomId,
                    Sender = sender!,
                });
            }

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChatTextHandler)} - {message.GetType()}");
    }
}
