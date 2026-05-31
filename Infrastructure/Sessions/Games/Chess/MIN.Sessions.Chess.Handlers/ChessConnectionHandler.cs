using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Chess.Messaging.Default;

namespace MIN.Sessions.Chess.Handlers;

internal sealed class ChessConnectionHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ChessConnectionHandler"/>
    /// </summary>
    public ChessConnectionHandler(ISubRoomManager subRoomManager,
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

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.ChessParticipantJoined, MessageTypeTag.ChessParticipantLeft];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case ChessParticipantJoinedMessage chessParticipantJoinedMessage:

                return HandlerResult.Success();

            case ChessParticipantLeftMessage chessParticipantLeftMessage:

                return HandlerResult.Success();

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ChessConnectionHandler)} - {message.GetType()}");
        }
    }
}
