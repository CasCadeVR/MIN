using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionSpecificHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly ISessionProcessTransport sessionProcessTransport;
    private readonly IRoomHoster roomHoster;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionSpecificHandler"/>
    /// </summary>
    public SessionSpecificHandler(ISubRoomManager subRoomManager,
        ISessionProcessTransport sessionProcessTransport,
        IRoomHoster roomHoster,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.sessionProcessTransport = sessionProcessTransport;
        this.roomHoster = roomHoster;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionSpecific];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionSpecificMessage sessionSpecificMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionLeaveHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionLeaveHandler)} - {message.GetType()}");
        }

        var roomId = context.RoomContext.RoomId;
        var subRoomId = sessionSpecificMessage.SubRoomId;

        await sessionProcessTransport.SendAsync(roomId, subRoomId, sessionSpecificMessage.SessionProcessRole, sessionSpecificMessage.Body, context.CancellationToken);

        return HandlerResult.Success();
    }
}
