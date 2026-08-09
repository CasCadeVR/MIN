using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallStateHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallStateHandler"/>
    /// </summary>
    public VoiceCallStateHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.VoiceStateRequest, MessageTypeTag.VoiceStateResponse];

    int IMessageHandler.Priority => 12;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
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
                logger.Log($"Получил инфу о текущем звонке: {voiceCallStateResponseMessage.ActiveSubRoomId ?? -1}");

                await eventBus.PublishAsync(new VoiceCallStateReceivedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    StartedAt = voiceCallStateResponseMessage.StartedAt,
                    ActiveSubRoomId = voiceCallStateResponseMessage.ActiveSubRoomId,
                    CallParticipants = context.RoomContext.Participants
                        .GetParticipantByIds(voiceCallStateResponseMessage.CallParticipantIds).ToList(),
                });

                return HandlerResult.Success();

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceCallStateHandler)} - {message.GetType()}");
        }
    }
}
