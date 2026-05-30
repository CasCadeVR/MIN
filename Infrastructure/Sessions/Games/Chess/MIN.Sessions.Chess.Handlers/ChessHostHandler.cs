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
using MIN.Sessions.Core.Messaging;

namespace MIN.Sessions.Chess.Handlers;

internal sealed class ChessHostHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IMessageRouter messageRouter;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChessHostHandler"/>
    /// </summary>
    public ChessHostHandler(ISubRoomManager subRoomManager,
        IMessageRouter messageRouter,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.messageRouter = messageRouter;
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

        // Здесь хост должен запустить сервер шахмат

        var somethingWentWrong = false;

        if (somethingWentWrong)
        {
            var hostFailedMessage = new SessionHostFailedMessage()
            {
                ErrorMessage = "Что-то пошло не так"
            };

            return HandlerResult.WithResponse(hostFailedMessage);
        }

        var senderParicipantInfo = sender!.ToParticipantInfo();

        var subRoomInfo = subRoomManager.HostSubRoom(context.RoomContext.RoomId, senderParicipantInfo, SubRoomPurpose.Activity);

        var hostReadyMessage = new SessionReadyMessage()
        {
            RoomId = context.RoomContext.RoomId,
            SubRoomId = subRoomInfo.Id,
            CurrentParticipantAmount = 1,
            Session = ChessSessionProvider.GetChessSession(),
            Sender = senderParicipantInfo,
        };

        await messageRouter.RouteAsync(hostReadyMessage, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);

        return HandlerResult.Success();
    }
}
