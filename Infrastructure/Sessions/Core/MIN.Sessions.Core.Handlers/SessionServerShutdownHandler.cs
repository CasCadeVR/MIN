using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.Contracts.Enums;
using MIN.Sessions.Core.Transport.Contracts.Models;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionServerShutdownHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly ISessionProcessManager sessionProcessManager;
    private readonly IMessageSender messageSender;
    private readonly INetworkErrorHandler networkErrorHandler;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionServerShutdownHandler"/>
    /// </summary>
    public SessionServerShutdownHandler(ISubRoomManager subRoomManager,
        ISessionProcessManager sessionProcessManager,
        IMessageSender messageSender,
        INetworkErrorHandler networkErrorHandler,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.sessionProcessManager = sessionProcessManager;
        this.messageSender = messageSender;
        this.networkErrorHandler = networkErrorHandler;
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

        await sessionProcessManager.StopAsync(new ProcessContext(roomId, subRoomId, SessionProcessRole.Client));

        if (context.Role == Role.Host)
        {
            var outOfSubRoomParticipants = context.RoomContext.Participants.GetParticipants()
                .Select(x => x.Id).Except(subRoomManager.GetParticipantIds(roomId, subRoomId)).ToList();

            if (!subRoomManager.TryStopSubRoom(roomId, subRoomId, sessionServerShutdownMessage.SenderId))
            {
                await networkErrorHandler.SendErrorAsync(
                    "Произошла попытка остановки сервера участником, не имеющего на это права, либо отправившего неккоректный id подкомнаты",
                    message.SenderId, roomId);
                return HandlerResult.Success();
            }

            var informConnectionIds = outOfSubRoomParticipants.Select(context.RoomContext.Connections.GetConnectionIdFromParticipantId);
            await messageSender.BroadcastAsync(sessionServerShutdownMessage, roomId, informConnectionIds, context.CancellationToken);
            return HandlerResult.Success(stopPropagation: true);
        }

        return HandlerResult.Failure($"Хост остановил сервер сессии: {sessionServerShutdownMessage.Reason}", stopPropagation: true);
    }
}
