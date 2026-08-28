using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallLeaveHandler : BaseHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IMessageRouter messageRouter;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallLeaveHandler"/>
    /// </summary>
    public VoiceCallLeaveHandler(ISubRoomManager subRoomManager,
        IMessageRouter messageRouter,
        ILoggerProvider logger) : base(logger)
    {
        this.subRoomManager = subRoomManager;
        this.messageRouter = messageRouter;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.VoiceCallLeave];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var voiceCallLeaveMessage = (VoiceCallLeaveMessage)message;

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
        }, roomId, context.SelfId, context.CancellationToken);

        if (!subRoomManager.LeaveSubRoom(roomId, voiceCallLeaveMessage.SubRoomId, message.SenderId))
        {
            await messageRouter.RouteAsync(new VoiceCallEndedMessage()
            {
                SubRoomId = voiceCallLeaveMessage.SubRoomId,
            }, roomId, context.SelfId, context.CancellationToken);
        }

        return HandlerResult.Success();
    }
}
