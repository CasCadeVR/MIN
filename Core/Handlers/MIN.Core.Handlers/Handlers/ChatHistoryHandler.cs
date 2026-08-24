using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Exceptions;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.History;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ChatHistoryHandler : BaseHandler
{
    private readonly IRoomStore roomStore;

    public ChatHistoryHandler(IRoomStore roomStore, ILoggerProvider logger) : base(logger)
    {
        this.roomStore = roomStore;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes
        => [MessageTypeTag.ChatHistoryRequest, MessageTypeTag.ChatHistoryResponse];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var roomId = context.RoomContext.RoomId;

        switch (message)
        {
            case ChatHistoryRequestMessage request:
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

            case ChatHistoryResponseMessage response:
                if (roomStore.GetRoomHostParticipantId(roomId) == response.SenderId
                    && context.Role == Role.Client)
                {
                    foreach (var roomMessage in response.Messages)
                    {
                        context.RoomContext.Messages.AddMessage(roomMessage, appendOnStart: true);
                    }
                }

                return HandlerResult.WithEvent(new ChatHistoryUpdatedEvent()
                {
                    RoomId = roomId,
                    Message = response,
                });

            default:
                throw new HandlerTypeMismatch(this, message);
        }
    }
}
