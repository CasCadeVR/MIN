using MIN.Services.Contracts.Interfaces;
using MIN.Events.Contracts;
using MIN.Events.Events;
using MIN.Handlers.Contracts;
using MIN.Handlers.Contracts.Models;
using MIN.Messaging.Contracts;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Messaging.Stateless.RoomRelated;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="RoomInfoRequestMessage"/> и <see cref="RoomInfoResponseMessage"/>
/// </summary>
internal sealed class RoomInfoHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IRoomService roomService;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="HandshakeHandler"/>
    /// </summary>
    public RoomInfoHandler(IRoomService roomService, IEventBus eventBus, ILoggerProvider logger)
    {
        this.roomService = roomService;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.RoomInfoRequest, MessageTypeTag.RoomInfoResponse];

    int IMessageHandler.Priority => 1;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is RoomInfoRequestMessage roomInfoRequest)
        {
            var room = roomService.GetRoom(roomInfoRequest.RoomId);
            var response = new RoomInfoResponseMessage()
            {
                Room = room
            };

            logger.Log($"Отправил информацию о комнате с id {roomInfoRequest.RoomId}");

            return HandlerResult.WithResponse(response);
        }
        else if (message is RoomInfoResponseMessage roomInfoResponse)
        {
            roomService.SetRoom(context.ConnectionId, roomInfoResponse.Room);
            logger.Log($"Получил информацию о комнате с id {roomInfoResponse.Room.Id}");

            await eventBus.PublishAsync(new RoomStateChangedEvent()
            {
                Room = roomInfoResponse.Room,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(RoomInfoHandler)} - {message.GetType()}");
    }
}
