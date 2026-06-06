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
using MIN.Sessions.Chess.Messaging.Default;
using MIN.Sessions.Chess.Services.Contracts.Services;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging;
using MIN.Sessions.Core.Messaging.Contracts;

namespace MIN.Sessions.Chess.Handlers;

internal sealed class ChessJoinHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChessJoinHandler"/>
    /// </summary>
    public ChessJoinHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.ChessJoinRequest, MessageTypeTag.ChessJoinResponse];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case ChessJoinRequestMessage chessJoinRequestMessage:
                if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
                {
                    return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
                }

                var roomId = context.RoomContext.RoomId;

                var subRoomInfo = subRoomManager.GetSubRoom(roomId, chessJoinRequestMessage.SubRoomId);

                if (subRoomInfo == null)
                {
                    return SendErrorMessage("Такая подкомната не нашлась");
                }

                var senderParicipantInfo = sender!.ToParticipantInfo();

                var joinResult = subRoomManager.TryJoinSubRoom(roomId, chessJoinRequestMessage.SubRoomId, senderParicipantInfo);

                if (joinResult != SubRoomJoinOutcome.Success)
                {
                    if (joinResult == SubRoomJoinOutcome.SubRoomNotActive)
                    {
                        await messageRouter.RouteAsync(new ChessHostRequestMessage()
                        {
                            RoomId = roomId,
                            Options = null,
                            SubRoomId = subRoomInfo.Id,
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

                return HandlerResult.WithResponse(new ChessJoinResponseMessage()
                {
                    NeedToAnnounce = true,
                    SubRoomId = subRoomInfo.Id,
                    CurrentPositionOnBoard = "And at this position Magnus carlson plays ROOK D7",
                });

            case ChessJoinResponseMessage chessJoinResponseMessage:
                await eventBus.PublishAsync(new JoinResponseReceivedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    SubRoomId = chessJoinResponseMessage.SubRoomId,
                    Session = ChessSessionProvider.GetChessSession(),
                });

                if (!chessJoinResponseMessage.NeedToAnnounce)
                {
                    return HandlerResult.Success();
                }

                var selfParticipant = identityService.SelfParticipant.ToParticipantInfo();

                var sessionParticipantJoinedMessage = new SessionParticipantJoinedMessage()
                {
                    SubRoomId = chessJoinResponseMessage.SubRoomId,
                    Participant = selfParticipant,
                };

                await messageRouter.RouteAsync(sessionParticipantJoinedMessage, context.RoomContext.RoomId, selfParticipant.Id, context.CancellationToken);

                return HandlerResult.WithResponse(new ChessParticipantJoinedMessage()
                {
                    Participant = selfParticipant,
                    SubRoomId = chessJoinResponseMessage.SubRoomId,
                });

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChessJoinHandler)} - {message.GetType()}");
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
