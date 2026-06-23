using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
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
    private readonly ISessionScanner sessionScanner;
    private readonly INetworkErrorHandler networkErrorHandler;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionJoinHandler"/>
    /// </summary>
    public SessionJoinHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        ISessionScanner sessionScanner,
        INetworkErrorHandler networkErrorHandler,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.sessionScanner = sessionScanner;
        this.networkErrorHandler = networkErrorHandler;
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
                    await networkErrorHandler.SendErrorAsync("Такая подкомната не нашлась", message.SenderId, roomId);
                    return HandlerResult.Success();
                }

                var session = sessionScanner.GetSessionById(sessionJoinRequestMessage.SessionId);

                if (session == null)
                {
                    await networkErrorHandler.SendErrorAsync("У хоста не установлена программа сервера этой сессии", message.SenderId, context.RoomContext.RoomId);
                    return HandlerResult.Success();
                }

                if (session.Version != sessionJoinRequestMessage.SessionVersion)
                {
                    var clientOnOlderVersion = session.Version > sessionJoinRequestMessage.SessionVersion ? "Вы" : "Хост";
                    await networkErrorHandler.SendErrorAsync($"{clientOnOlderVersion} на устаревшей версии сессии: " +
                        $"\nВаша версия сессии - {sessionJoinRequestMessage.SessionVersion}" +
                        $"\nВерсия сессии хоста комнаты - {session.Version}", message.SenderId, context.RoomContext.RoomId);
                    return HandlerResult.Success();
                }

                if (session.MaximumParticipants.HasValue && subRoomInfo.Participants.Count >= session.MaximumParticipants)
                {
                    await networkErrorHandler.SendErrorAsync($"Сессия {session.Name} уже заполнена", message.SenderId, context.RoomContext.RoomId);
                    return HandlerResult.Success();
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
                            SessionId = sessionJoinRequestMessage.SessionId,
                            SessionVersion = sessionJoinRequestMessage.SessionVersion,
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

                    await networkErrorHandler.SendErrorAsync(error, message.SenderId, roomId);
                    return HandlerResult.Success();
                }

                return HandlerResult.WithResponse(new SessionJoinResponseMessage()
                {
                    NeedToAnnounce = true,
                    SubRoomId = subRoomInfo.Id,
                    SessionId = sessionJoinRequestMessage.SessionId
                });

            case SessionJoinResponseMessage sessionJoinResponseMessage:
                var responseSession = sessionScanner.GetSessionById(sessionJoinResponseMessage.SessionId);

                if (responseSession == null)
                {
                    return HandlerResult.Failure($"У вас не установлена сессия с id {sessionJoinResponseMessage.SessionId}");
                }

                await eventBus.PublishAsync(new SessionJoinResponseReceivedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    SubRoomId = sessionJoinResponseMessage.SubRoomId,
                    Session = responseSession,
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
}
