using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallLeaveHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IMessageRouter messageRouter;
    private readonly ILoggerProvider logger;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallLeaveHandler"/>
    /// </summary>
    public VoiceCallLeaveHandler(ISubRoomManager subRoomManager,
        IMessageRouter messageRouter,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.messageRouter = messageRouter;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.VoiceCallLeave];

    int IMessageHandler.Priority => 12;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not VoiceCallLeaveMessage voiceCallLeaveMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(VoiceCallLeaveHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceCallLeaveHandler)} - {message.GetType()}");
        }

        if (context.Role != Role.Host)
        {
            return HandlerResult.Failure($"Получил сообщение {message.GetType()} в {nameof(VoiceCallLeaveHandler)} как {context.Role}, хотя не должен был", stopPropagation: false);
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
        }

        var roomId = context.RoomContext.RoomId;

        if (subRoomManager.GetSubRoom(roomId, voiceCallLeaveMessage.SubRoomId) == null)
        {
            return HandlerResult.Failure("Клиент отправил запрос на выход из неизвестной сессии", stopPropagation: true);
        }

        await messageRouter.RouteAsync(new VoiceParticipantLeftMessage()
        {
            SubRoomId = voiceCallLeaveMessage.SubRoomId,
            Participant = sender!.ToParticipantInfo(),
        }, roomId, identityService.SelfParticipant.Id, context.CancellationToken);

        if (!subRoomManager.LeaveSubRoom(roomId, voiceCallLeaveMessage.SubRoomId, message.SenderId))
        {
            await messageRouter.RouteAsync(new VoiceCallEndedMessage()
            {
                SubRoomId = voiceCallLeaveMessage.SubRoomId,
            }, roomId, identityService.SelfParticipant.Id, context.CancellationToken);
        }

        return HandlerResult.Success();
    }
}
