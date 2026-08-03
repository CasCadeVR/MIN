using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.History;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ChatHistoryHandler : IMessageHandler
{
    private readonly IRoomStore roomStore;
    private readonly IEventBus eventBus;

    public ChatHistoryHandler(IRoomStore roomStore, IEventBus eventBus)
    {
        this.roomStore = roomStore;
        this.eventBus = eventBus;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.ChatHistoryRequest, MessageTypeTag.ChatHistoryResponse];

    int IMessageHandler.Priority => 2;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        var roomId = context.RoomContext.RoomId;

        if (message is ChatHistoryRequestMessage request)
        {
            var totalCount = roomStore.GetRoomChatHistoryCountFor(request.SenderId, roomId);
            var pageMessages = context.RoomContext
                .Messages
                .GetRecentHistory(request.Page, request.PageSize)
                .ToList();

            return HandlerResult.WithResponse(new ChatHistoryResponseMessage
            {
                Messages = pageMessages,
                TotalCount = totalCount,
                Page = request.Page,
            }, stopPropagation: true);
        }
        else if (message is ChatHistoryResponseMessage response)
        {
            if (roomStore.GetRoomHostParticipantId(roomId) == response.SenderId
                && context.Role == Role.Client)
            {
                foreach (var roomMessage in response.Messages)
                {
                    context.RoomContext.Messages.AddMessage(roomMessage, appendOnStart: true);
                }
            }

            await eventBus.PublishAsync(new ChatHistoryUpdatedEvent()
            {
                RoomId = roomId,
                Message = response,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChatHistoryHandler)} - {message.GetType()}");
    }
}
