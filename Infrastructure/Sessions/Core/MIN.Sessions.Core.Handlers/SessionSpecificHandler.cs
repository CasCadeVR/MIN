using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionSpecificHandler : IMessageHandler
{
    private readonly ISessionProcessBridge sessionProcessBridge;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionSpecificHandler"/>
    /// </summary>
    public SessionSpecificHandler(ISessionProcessBridge sessionProcessBridge, ILoggerProvider logger)
    {
        this.sessionProcessBridge = sessionProcessBridge;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionSpecific];

    int IMessageHandler.Priority => 2;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionSpecificMessage sessionSpecificMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionLeaveHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionLeaveHandler)} - {message.GetType()}");
        }

        var roomId = context.RoomContext.RoomId;
        var subRoomId = sessionSpecificMessage.SubRoomId;

        await sessionProcessBridge.SendData(sessionSpecificMessage.Body, new ProcessContext(roomId, subRoomId, sessionSpecificMessage.SessionProcessRole), context.CancellationToken);
        return HandlerResult.Success();
    }
}
