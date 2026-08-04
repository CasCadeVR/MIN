using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Moderation;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.Ipc;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.Contracts.Enums;
using MIN.Sessions.Core.Transport.Contracts.Models;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionHostHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageSender messageSender;
    private readonly IMessageRouter messageRouter;
    private readonly ISessionScanner sessionScanner;
    private readonly ISessionProcessBridge sessionProcessBridge;
    private readonly ISessionProcessManager sessionProcessManager;
    private readonly INetworkErrorHandler networkErrorHandler;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionHostHandler"/>
    /// </summary>
    public SessionHostHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageSender messageSender,
        IMessageRouter messageRouter,
        ISessionScanner sessionScanner,
        ISessionProcessBridge sessionProcessBridge,
        ISessionProcessManager sessionProcessManager,
        INetworkErrorHandler networkErrorHandler,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageSender = messageSender;
        this.messageRouter = messageRouter;
        this.sessionScanner = sessionScanner;
        this.sessionProcessBridge = sessionProcessBridge;
        this.sessionProcessManager = sessionProcessManager;
        this.networkErrorHandler = networkErrorHandler;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionHostRequest];

    int IMessageHandler.Priority => 12;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionHostRequestMessage sessionHostRequestMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionHostHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionHostHandler)} - {message.GetType()}");
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
        }

        var senderParicipantInfo = sender!.ToParticipantInfo();

        if (sessionHostRequestMessage.SubRoomId != null
            && !subRoomManager.ActivateSubRoom(context.RoomContext.RoomId, sessionHostRequestMessage.SubRoomId.Value, senderParicipantInfo))
        {
            await networkErrorHandler.SendErrorAsync("Хост не смог создать комнату", message.SenderId, context.RoomContext.RoomId);
            return HandlerResult.Success();
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
            await networkErrorHandler.SendErrorAsync("У хоста не установлена программа сервера этой сессии", message.SenderId, context.RoomContext.RoomId);
            return HandlerResult.Success();
        }

        if (session.Version != sessionHostRequestMessage.SessionVersion)
        {
            var clientOnOlderVersion = session.Version > sessionHostRequestMessage.SessionVersion ? "Вы" : "Хост";
            await networkErrorHandler.SendErrorAsync($"{clientOnOlderVersion} на устаревшей версии сессии: " +
                $"\nВаша версия сессии - {sessionHostRequestMessage.SessionVersion}" +
                $"\nВерсия сессии хоста комнаты - {session.Version}", message.SenderId, context.RoomContext.RoomId);
            return HandlerResult.Success();
        }

        var processContext = new ProcessContext(context.RoomContext.RoomId, subRoomId.Value, SessionProcessRole.Server);

        var hostResult = await sessionProcessManager.StartAsync(session,
            processContext, context.CancellationToken);

        if (hostResult == false)
        {
            await networkErrorHandler.SendErrorAsync("У хоста повреждёна или утеряна программа сервера", message.SenderId, context.RoomContext.RoomId);
            return HandlerResult.Success();
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

            if (identityService.SelfParticipant.Id == message.SenderId)
            {
                await eventBus.PublishAsync(new SessionJoinResponseReceivedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    SubRoomId = subRoomId.Value,
                    Session = session,
                });

                return HandlerResult.Success();
            }
            else
            {
                await messageSender.SendAsync(hostReadyMessage, context.RoomContext.RoomId, context.ConnectionId, context.CancellationToken);

                return HandlerResult.WithResponse(new SessionJoinResponseMessage()
                {
                    NeedToAnnounce = false,
                    SessionId = session.SessionId,
                    SubRoomId = subRoomId.Value,
                });
            }
        }

        if (identityService.SelfParticipant.Id == message.SenderId)
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
