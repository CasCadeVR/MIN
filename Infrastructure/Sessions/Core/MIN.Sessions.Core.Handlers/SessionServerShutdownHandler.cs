using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionServerShutdownHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly ISessionProcessManager sessionProcessInitializer;
    private readonly IRoomHoster roomHoster;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionServerShutdownHandler"/>
    /// </summary>
    public SessionServerShutdownHandler(ISubRoomManager subRoomManager,
        ISessionProcessManager sessionProcessInitializer,
        IRoomHoster roomHoster,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.sessionProcessInitializer = sessionProcessInitializer;
        this.roomHoster = roomHoster;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionServerShutdown];

    int IMessageHandler.Priority => 8;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionServerShutdownMessage sessionServerShutdownMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionServerShutdownHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionServerShutdownHandler)} - {message.GetType()}");
        }

        var roomId = context.RoomContext.RoomId;
        var subRoomId = sessionServerShutdownMessage.SubRoomId;

        if (roomHoster.IsHosting(roomId))
        {
            if (!subRoomManager.TryStopSubRoom(roomId, subRoomId, sessionServerShutdownMessage.SenderId))
            {
                return HandlerResult.WithResponse(new SessionHostFailedMessage()
                {
                    ErrorMessage = "Произошла попытка остановки сервера участником, не имеющего на это права, либо отправившего неккоректный id подкомнаты",
                });
            }
        }

        await sessionProcessInitializer.StopAsync(new ProcessContext(roomId, subRoomId, SessionProcessRole.Client));

        return HandlerResult.Success();
    }
}
