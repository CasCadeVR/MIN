using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.Ipc;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.Contracts.Enums;
using MIN.Sessions.Core.Transport.Contracts.Models;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionHostHandler : BaseHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IMessageSender messageSender;
    private readonly IMessageRouter messageRouter;
    private readonly ISessionScanner sessionScanner;
    private readonly ISessionProcessBridge sessionProcessBridge;
    private readonly ISessionProcessManager sessionProcessManager;

    public SessionHostHandler(ISubRoomManager subRoomManager,
        IMessageSender messageSender,
        IMessageRouter messageRouter,
        ISessionScanner sessionScanner,
        ISessionProcessBridge sessionProcessBridge,
        ISessionProcessManager sessionProcessManager,
        ILoggerProvider logger) : base(logger)
    {
        this.subRoomManager = subRoomManager;
        this.messageSender = messageSender;
        this.messageRouter = messageRouter;
        this.sessionScanner = sessionScanner;
        this.sessionProcessBridge = sessionProcessBridge;
        this.sessionProcessManager = sessionProcessManager;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.SessionHostRequest];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var sessionHostRequestMessage = (SessionHostRequestMessage)message;

        if (context.Role != Role.Host)
        {
            return HandlerResult.Failure($"Получил сообщение {message.GetType()} в {nameof(SessionHostHandler)} как {context.Role}, хотя не должен был", stopPropagation: false);
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
        }

        var senderParicipantInfo = sender!.ToParticipantInfo();

        if (sessionHostRequestMessage.SubRoomId != null
            && !subRoomManager.ActivateSubRoom(context.RoomContext.RoomId, sessionHostRequestMessage.SubRoomId.Value, senderParicipantInfo))
        {
            return HandlerResult.WithErrorHandled("Хост не смог создать комнату");
        }

        var subRoomId = sessionHostRequestMessage.SubRoomId;
        var isHosted = false;

        if (subRoomId == null)
        {
            var subRoomInfo = subRoomManager.HostSubRoom(context.RoomContext.RoomId, senderParicipantInfo, SubRoomPurpose.Activity);
            isHosted = true;
            subRoomId = subRoomInfo.Id;
        }

        var session = sessionScanner.GetSessionById(sessionHostRequestMessage.SessionId);

        if (session == null)
        {
            subRoomManager.TryStopSubRoom(context.RoomContext.RoomId, subRoomId.Value, message.SenderId);
            return HandlerResult.WithErrorHandled("У хоста не установлена программа сервера этой сессии");
        }

        if (session.Version != sessionHostRequestMessage.SessionVersion)
        {
            subRoomManager.TryStopSubRoom(context.RoomContext.RoomId, subRoomId.Value, message.SenderId);
            var clientOnOlderVersion = session.Version > sessionHostRequestMessage.SessionVersion ? "Вы" : "Хост";
            return HandlerResult.WithErrorHandled($"{clientOnOlderVersion} на устаревшей версии сессии: " +
                $"\nВаша версия сессии - {sessionHostRequestMessage.SessionVersion}" +
                $"\nВерсия сессии хоста комнаты - {session.Version}");
        }

        var processContext = new ProcessContext(context.RoomContext.RoomId, subRoomId.Value, SessionProcessRole.Server);

        var hostResult = await sessionProcessManager.StartAsync(session,
            processContext, context.CancellationToken);

        if (hostResult == false)
        {
            subRoomManager.TryStopSubRoom(context.RoomContext.RoomId, subRoomId.Value, message.SenderId);
            return HandlerResult.WithErrorHandled("У хоста повреждёна или утеряна программа сервера");
        }

        if (isHosted)
        {
            var hostReadyMessage = new SessionReadyMessage()
            {
                SubRoomId = subRoomId.Value,
                CurrentParticipantAmount = 1,
                Session = session,
                Sender = senderParicipantInfo,
                SenderId = message.SenderId,
                ThumbnailData = sessionScanner.LoadThumbnail(session.SessionId)
            };

            await sessionProcessBridge.SendIpcMessage(new ParticipantConnectedMessage(senderParicipantInfo.Id.ToString(), senderParicipantInfo.Name),
                processContext, message.SenderId, context.CancellationToken);

            await messageRouter.RouteAsync(hostReadyMessage, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);

            if (context.SelfId == message.SenderId)
            {
                return HandlerResult.WithEvent(new SessionJoinResponseReceivedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    SubRoomId = subRoomId.Value,
                    Session = session,
                });
            }
            else
            {
                // sending him ready as he didnt received by sender filtering
                await messageSender.SendAsync(hostReadyMessage, context.RoomContext.RoomId, context.ConnectionId, context.CancellationToken);

                return HandlerResult.WithResponse(new SessionJoinResponseMessage()
                {
                    NeedToAnnounce = false,
                    SessionId = session.SessionId,
                    SubRoomId = subRoomId.Value,
                });
            }
        }

        if (context.SelfId == message.SenderId)
        {
            await messageRouter.RouteAsync(new SessionJoinResponseMessage()
            {
                NeedToAnnounce = true,
                SessionId = session.SessionId,
                SubRoomId = subRoomId.Value,
            }, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);
        }
        else
        {
            await messageSender.SendAsync(new SessionJoinResponseMessage()
            {
                NeedToAnnounce = true,
                SessionId = session.SessionId,
                SubRoomId = subRoomId.Value,
            }, context.RoomContext.RoomId, context.RoomContext.Connections.GetConnectionIdFromParticipantId(message.SenderId), context.CancellationToken);
        }

        return HandlerResult.Success();
    }
}
