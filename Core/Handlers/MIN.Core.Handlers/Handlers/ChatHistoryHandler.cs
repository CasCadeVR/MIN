using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.History;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ChatHistoryHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IRoomStore roomStore;
    private readonly IRoomHoster roomHoster;
    private readonly IEventBus eventBus;

    public ChatHistoryHandler(IRoomStore roomStore,
        IRoomHoster roomHoster,
        IEventBus eventBus)
    {
        this.roomStore = roomStore;
        this.roomHoster = roomHoster;
        this.eventBus = eventBus;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.ChatHistoryRequest, MessageTypeTag.ChatHistoryResponse];

    int IMessageHandler.Priority => 2;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is ChatHistoryRequestMessage request)
        {
            var room = roomStore.GetRoomFor(request.SenderId, request.RoomId);
            var totalCount = room.TotalMessageCount;

            var pageMessages = context.RoomContext
                .Messages
                .GetRecentHistory(request.Page, request.PageSize)
                .ToList();

            return HandlerResult.WithResponse(new ChatHistoryResponseMessage
            {
                RoomId = request.RoomId,
                Messages = pageMessages,
                TotalCount = totalCount,
                Page = request.Page,
            }, stopPropagation: true);
        }
        else if (message is ChatHistoryResponseMessage response)
        {
            if (!roomHoster.IsHosting(context.RoomContext.RoomId))
            {
                foreach (var roomMessage in response.Messages)
                {
                    context.RoomContext.Messages.AddMessage(roomMessage);
                }
            }

            await eventBus.PublishAsync(new ChatHistoryUpdatedEvent()
            {
                RoomId = context.RoomContext.RoomId,
                Message = response,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChatHistoryHandler)} - {message.GetType()}");
    }
}
