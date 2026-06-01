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
using MIN.Sessions.Chess.Events;
using MIN.Sessions.Chess.Messaging.Default;
using MIN.Sessions.Chess.Services.Contracts.Services;
using MIN.Sessions.Core.Messaging;

namespace MIN.Sessions.Chess.Handlers;

internal sealed class ChessHostHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageSender messageSender;
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChessHostHandler"/>
    /// </summary>
    public ChessHostHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageSender messageSender,
        IMessageRouter messageRouter,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageSender = messageSender;
        this.messageRouter = messageRouter;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.ChessHostRequest];

    int IMessageHandler.Priority => 12;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not ChessHostRequestMessage chessHostRequestMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(ChessHostHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChessHostHandler)} - {message.GetType()}");
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
        }

        var senderParicipantInfo = sender!.ToParticipantInfo();

        if (chessHostRequestMessage.SubRoomId != null
            && !subRoomManager.ActivateSubRoom(context.RoomContext.RoomId, chessHostRequestMessage.SubRoomId.Value, senderParicipantInfo))
        {
            return HandlerResult.WithResponse(new SessionHostFailedMessage()
            {
                ErrorMessage = "Что-то пошло не так"
            });
        }

        // Здесь хост должен запустить сервер шахмат

        var currentPositionOnBoard = "And at this position Magnus carlson plays ROOK D7";

        var subRoomId = chessHostRequestMessage.SubRoomId;

        if (subRoomId == null)
        {
            var subRoomInfo = subRoomManager.HostSubRoom(context.RoomContext.RoomId, senderParicipantInfo, SubRoomPurpose.Activity);
            subRoomId = subRoomInfo.Id;
            var hostReadyMessage = new SessionReadyMessage()
            {
                SubRoomId = subRoomInfo.Id,
                CurrentParticipantAmount = 1,
                Session = ChessSessionProvider.GetChessSession(),
                Sender = senderParicipantInfo,
                SenderId = message.SenderId,
            };

            await messageRouter.RouteAsync(hostReadyMessage, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);

            if (identityService.SelfParticipant.Id == message.SenderId)
            {
                await eventBus.PublishAsync(new ChessJoinResponseReceivedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    SubRoomId = subRoomInfo.Id,
                    CurrentPositionOnBoard = currentPositionOnBoard,
                });

                return HandlerResult.Success();
            }
            else
            {
                await messageSender.SendAsync(hostReadyMessage, context.RoomContext.RoomId, context.ConnectionId, context.CancellationToken);

                return HandlerResult.WithResponse(new ChessJoinResponseMessage()
                {
                    NeedToAnnounce = false,
                    SubRoomId = subRoomId.Value,
                    CurrentPositionOnBoard = currentPositionOnBoard,
                });
            }
        }

        if (identityService.SelfParticipant.Id == message.SenderId)
        {
            await messageRouter.RouteAsync(new ChessJoinResponseMessage()
            {
                NeedToAnnounce = true,
                SubRoomId = subRoomId.Value,
                CurrentPositionOnBoard = currentPositionOnBoard,
            }, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);
        }
        else
        {
            await messageSender.SendAsync(new ChessJoinResponseMessage()
            {
                NeedToAnnounce = true,
                SubRoomId = subRoomId.Value,
                CurrentPositionOnBoard = currentPositionOnBoard,
            }, context.RoomContext.RoomId, context.RoomContext.Connections.GetConnectionIdFromParticipantId(message.SenderId), context.CancellationToken);
        }

        return HandlerResult.Success();
    }
}
