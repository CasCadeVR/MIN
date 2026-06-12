using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.Ipc;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionHostHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageSender messageSender;
    private readonly IMessageRouter messageRouter;
    private readonly ISessionResolver sessionResolver;
    private readonly ISessionProcessBridge sessionProcessBridge;
    private readonly ISessionProcessManager sessionProcessInitializer;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionHostHandler"/>
    /// </summary>
    public SessionHostHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageSender messageSender,
        IMessageRouter messageRouter,
        ISessionResolver sessionResolver,
        ISessionProcessBridge sessionProcessBridge,
        ISessionProcessManager sessionProcessInitializer,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageSender = messageSender;
        this.messageRouter = messageRouter;
        this.sessionResolver = sessionResolver;
        this.sessionProcessBridge = sessionProcessBridge;
        this.sessionProcessInitializer = sessionProcessInitializer;
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
            return HandlerResult.WithResponse(new SessionHostFailedMessage()
            {
                ErrorMessage = "Что-то пошло не так"
            });
        }

        // Здесь хост должен запустить сервер шахмат

        var subRoomId = sessionHostRequestMessage.SubRoomId;
        var isHosted = false;

        if (subRoomId == null)
        {
            var subRoomInfo = subRoomManager.HostSubRoom(context.RoomContext.RoomId, senderParicipantInfo, SubRoomPurpose.Activity);
            isHosted = true;
            subRoomId = subRoomInfo.Id;
        }

        var session = sessionResolver.GetSessionByType(sessionHostRequestMessage.SessionType);

        var processContext = new ProcessContext(context.RoomContext.RoomId, subRoomId.Value, SessionProcessRole.Server);

        var hostResult = await sessionProcessInitializer.StartAsync(session.ServerPath,
            processContext, context.CancellationToken);

        if (hostResult == false)
        {
            return HandlerResult.WithResponse(new SessionHostFailedMessage()
            {
                ErrorMessage = "У хоста повреждёна или утеряна программа сервера"
            });
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
            };

            await sessionProcessBridge.SendIpcMessage(new ParticipantConnectedMessage(senderParicipantInfo.Id.ToString(), senderParicipantInfo.Name), processContext, context.CancellationToken);

            await messageRouter.RouteAsync(hostReadyMessage, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);

            if (identityService.SelfParticipant.Id == message.SenderId)
            {
                await eventBus.PublishAsync(new JoinResponseReceivedEvent()
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
                    SubRoomId = subRoomId.Value,
                });
            }
        }

        if (identityService.SelfParticipant.Id == message.SenderId)
        {
            await messageRouter.RouteAsync(new SessionJoinResponseMessage()
            {
                NeedToAnnounce = true,
                SubRoomId = subRoomId.Value,
            }, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);
        }
        else
        {
            await messageSender.SendAsync(new SessionJoinResponseMessage()
            {
                NeedToAnnounce = true,
                SubRoomId = subRoomId.Value,
            }, context.RoomContext.RoomId, context.RoomContext.Connections.GetConnectionIdFromParticipantId(message.SenderId), context.CancellationToken);
        }

        return HandlerResult.Success();
    }
}
