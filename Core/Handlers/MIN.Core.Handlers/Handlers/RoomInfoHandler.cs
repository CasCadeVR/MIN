using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated.RoomInfo;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class RoomInfoHandler : IMessageHandler
{
    private readonly IRoomStore roomStore;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    public RoomInfoHandler(IRoomStore roomStore,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.roomStore = roomStore;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.RoomInfoRequest, MessageTypeTag.RoomInfoResponse, MessageTypeTag.RoomInfoUpdated];

    int IMessageHandler.Priority => 1;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        var roomId = context.RoomContext.RoomId;

        if (message is RoomInfoRequestMessage roomInfoRequest)
        {
            logger.Log($"Отправляю информацию о комнате с id {roomId}");
            return HandlerResult.WithResponse(new RoomInfoResponseMessage()
            {
                Room = roomStore.GetRoomFor(message.SenderId, roomId),
            });
        }
        else if (message is RoomInfoResponseMessage roomInfoResponse)
        {
            roomStore.Register(roomInfoResponse.Room);

            var history = roomInfoResponse.Room.ChatHistory.AsEnumerable().Reverse();
            foreach (var roomMessage in history)
            {
                context.RoomContext.Messages.AddMessage(roomMessage);
            }

            logger.Log($"Получил информацию о комнате с id {roomInfoResponse.Room.Id} сообщений {roomInfoResponse.Room.TotalMessageCount}");

            await eventBus.PublishAsync(new RoomStateChangedEvent()
            {
                Room = roomInfoResponse.Room,
            }, context.CancellationToken);

            await eventBus.PublishAsync(new RoomJoinedEvent()
            {
                RoomId = roomInfoResponse.Room.Id,
                RoomInfo = new RoomInfo(roomInfoResponse.Room),
            });

            return HandlerResult.Success();
        }
        else if (message is RoomInfoUpdatedMessage roomInfoUpdated)
        {
            var existingRoom = roomStore.GetRoom(roomId);
            existingRoom.Name = roomInfoUpdated.Room.Name;
            existingRoom.MaximumParticipants = roomInfoUpdated.Room.MaximumParticipants;

            await eventBus.PublishAsync(new RoomInfoUpdatedMessageEvent()
            {
                RoomInfo = roomInfoUpdated.Room,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(RoomInfoHandler)} - {message.GetType()}");
    }
}
