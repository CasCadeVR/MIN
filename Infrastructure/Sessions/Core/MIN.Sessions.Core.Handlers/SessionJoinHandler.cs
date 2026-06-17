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
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionJoinHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly ISessionResolver sessionResolver;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionJoinHandler"/>
    /// </summary>
    public SessionJoinHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        ISessionResolver sessionResolver,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.sessionResolver = sessionResolver;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionJoinRequest, MessageTypeTag.SessionJoinResponse];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case SessionJoinRequestMessage sessionJoinRequestMessage:
                if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
                {
                    return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
                }

                var roomId = context.RoomContext.RoomId;

                var subRoomInfo = subRoomManager.GetSubRoom(roomId, sessionJoinRequestMessage.SubRoomId);

                if (subRoomInfo == null)
                {
                    return SendErrorMessage("Такая подкомната не нашлась");
                }

                var senderParicipantInfo = sender!.ToParticipantInfo();

                var joinResult = subRoomManager.TryJoinSubRoom(roomId, sessionJoinRequestMessage.SubRoomId, senderParicipantInfo);

                if (joinResult != SubRoomJoinOutcome.Success)
                {
                    if (joinResult == SubRoomJoinOutcome.SubRoomNotActive)
                    {
                        await messageRouter.RouteAsync(new SessionHostRequestMessage()
                        {
                            SubRoomId = subRoomInfo.Id,
                            SessionType = sessionJoinRequestMessage.SessionType
                        }, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);

                        return HandlerResult.Success();
                    }

                    var error = joinResult switch
                    {
                        SubRoomJoinOutcome.RoomNotFound => "Комната не нашлась",
                        SubRoomJoinOutcome.SubRoomNotFound => "Нету информации о подкомнате",
                        SubRoomJoinOutcome.AlreadyJoined => "Вы уже учавствуете в этой сессии",
                        _ => "Не удалось войти"
                    };
                    return SendErrorMessage(error);
                }

                return HandlerResult.WithResponse(new SessionJoinResponseMessage()
                {
                    NeedToAnnounce = true,
                    SubRoomId = subRoomInfo.Id,
                    SessionType = sessionJoinRequestMessage.SessionType
                });

            case SessionJoinResponseMessage sessionJoinResponseMessage:
                await eventBus.PublishAsync(new JoinResponseReceivedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    SubRoomId = sessionJoinResponseMessage.SubRoomId,
                    Session = sessionResolver.GetSessionByType(sessionJoinResponseMessage.SessionType),
                });

                if (!sessionJoinResponseMessage.NeedToAnnounce)
                {
                    return HandlerResult.Success();
                }

                var selfParticipant = identityService.SelfParticipant.ToParticipantInfo();

                var sessionParticipantJoinedMessage = new SessionParticipantJoinedMessage()
                {
                    SubRoomId = sessionJoinResponseMessage.SubRoomId,
                    Participant = selfParticipant,
                };

                await messageRouter.RouteAsync(sessionParticipantJoinedMessage, context.RoomContext.RoomId, selfParticipant.Id, context.CancellationToken);

                return HandlerResult.Success();

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionJoinHandler)} - {message.GetType()}");
        }
    }

    private HandlerResult SendErrorMessage(string error)
    {
        return HandlerResult.WithResponse(new SessionJoinFailedMessage()
        {
            ErrorMessage = error,
        }, stopPropagation: true);
    }
}
