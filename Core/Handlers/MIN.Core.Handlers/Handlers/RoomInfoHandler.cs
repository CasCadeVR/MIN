using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.RoomRelated;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="RoomInfoRequestMessage"/>,
/// <see cref="RoomInfoResponseMessage"/>,
/// <see cref="RoomInfoUpdatedMessage"/>
/// </summary>
internal sealed class RoomInfoHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IRoomStore roomStore;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="HandshakeHandler"/>
    /// </summary>
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
        if (message is RoomInfoRequestMessage roomInfoRequest)
        {
            var room = roomStore.GetRoomFor(message.SenderId, roomInfoRequest.RoomId);

            var response = new RoomInfoResponseMessage()
            {
                Room = room,
            };

            logger.Log($"Отправил информацию о комнате с id {roomInfoRequest.RoomId}");

            return HandlerResult.WithResponse(response);
        }
        else if (message is RoomInfoResponseMessage roomInfoResponse)
        {
            roomStore.Register(roomInfoResponse.Room);
            foreach (var roomMessage in roomInfoResponse.Room.ChatHistory)
            {
                context.RoomContext.Messages.AddMessage(message);
            }

            foreach (var roomParticipant in roomInfoResponse.Room.CurrentParticipants)
            {
                context.RoomContext.Participants.AddParticipant(roomParticipant);
            }

            logger.Log($"Получил информацию о комнате с id {roomInfoResponse.Room.Id}");

            await eventBus.PublishAsync(new RoomStateChangedEvent()
            {
                Room = roomInfoResponse.Room,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }
        else if (message is RoomInfoUpdatedMessage roomInfoUpdated)
        {
            var existingRoom = roomStore.GetRoom(context.RoomContext.RoomId);
            existingRoom.Name = roomInfoUpdated.Room.Name;
            existingRoom.MaximumParticipants = roomInfoUpdated.Room.MaximumParticipants;

            await eventBus.PublishAsync(new RoomInfoChangedEvent()
            {
                Room = roomInfoUpdated.Room,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(RoomInfoHandler)} - {message.GetType()}");
    }
}
