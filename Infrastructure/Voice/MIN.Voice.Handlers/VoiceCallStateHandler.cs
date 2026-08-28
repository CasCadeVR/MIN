using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Exceptions;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallStateHandler : BaseHandler
{
    private readonly ISubRoomManager subRoomManager;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallStateHandler"/>
    /// </summary>
    public VoiceCallStateHandler(ISubRoomManager subRoomManager,
        ILoggerProvider logger) : base(logger)
    {
        this.subRoomManager = subRoomManager;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes
        => [MessageTypeTag.VoiceStateRequest, MessageTypeTag.VoiceStateResponse];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case VoiceCallStateRequestMessage _:
                if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
                {
                    return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
                }

                if (context.Role != Role.Host)
                {
                    return HandlerResult.Failure($"Получил сообщение {message.GetType()} в {nameof(VoiceCallStateHandler)} как {context.Role}, хотя не должен был", stopPropagation: false);
                }

                var roomId = context.RoomContext.RoomId;
                var allSubrooms = subRoomManager.GetRoomSubRooms(roomId);
                var voiceCallSubroom = allSubrooms.FirstOrDefault(x => x.Purpose == SubRoomPurpose.Voice && x.IsActive);

                var response = new VoiceCallStateResponseMessage()
                {
                    ActiveSubRoomId = voiceCallSubroom?.Id,
                };

                if (voiceCallSubroom != null)
                {
                    response.StartedAt = voiceCallSubroom.CreatedAt;
                    response.CallParticipantIds = voiceCallSubroom.Participants.Select(x => x.Id).ToList();
                }

                return HandlerResult.WithResponse(response);

            case VoiceCallStateResponseMessage voiceCallStateResponseMessage:
                LogInfo($"Получил инфу о текущем звонке: {voiceCallStateResponseMessage.ActiveSubRoomId ?? -1}");

                return HandlerResult.WithEvent(new VoiceCallStateReceivedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    StartedAt = voiceCallStateResponseMessage.StartedAt,
                    ActiveSubRoomId = voiceCallStateResponseMessage.ActiveSubRoomId,
                    CallParticipants = context.RoomContext.Participants
                        .GetParticipantByIds(voiceCallStateResponseMessage.CallParticipantIds).ToList(),
                });

            default:
                throw new HandlerTypeMismatch(this, message);
        }
    }
}
