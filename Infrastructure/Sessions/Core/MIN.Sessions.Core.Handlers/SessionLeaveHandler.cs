using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionLeaveHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IMessageRouter messageRouter;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionLeaveHandler"/>
    /// </summary>
    public SessionLeaveHandler(ISubRoomManager subRoomManager,
        IMessageRouter messageRouter,
        IEventBus eventBus,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.messageRouter = messageRouter;
        this.eventBus = eventBus;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionLeave];

    int IMessageHandler.Priority => 12;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionLeaveMessage sessionLeaveMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionLeaveHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionLeaveHandler)} - {message.GetType()}");
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
        }

        var roomId = context.RoomContext.RoomId;

        if (subRoomManager.GetSubRoom(roomId, sessionLeaveMessage.SubRoomId) == null)
        {
            return HandlerResult.Failure("Клиент отправил запрос на выход из неизвестной сессии", stopPropagation: true);
        }

        var isLast = false;

        if (!subRoomManager.LeaveSubRoom(roomId, sessionLeaveMessage.SubRoomId, message.SenderId))
        {
            await eventBus.PublishAsync(new SessionDeactivatedEvent()
            {
                RoomId = roomId,
                SubRoomId = sessionLeaveMessage.SubRoomId,
            });

            isLast = true;
        }

        var leaveMessage = new SessionParticipantLeftMessage()
        {
            SubRoomId = sessionLeaveMessage.SubRoomId,
            Participant = sender!.ToParticipantInfo(),
            IsLast = isLast
        };

        await messageRouter.RouteAsync(leaveMessage, roomId, identityService.SelfParticipant.Id, context.CancellationToken);
        return HandlerResult.Success();
    }
}
