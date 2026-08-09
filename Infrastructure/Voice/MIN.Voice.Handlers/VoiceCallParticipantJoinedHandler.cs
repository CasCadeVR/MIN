using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallParticipantJoinedHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly IVoicePlaybackService voicePlaybackService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallParticipantJoinedHandler"/>
    /// </summary>
    public VoiceCallParticipantJoinedHandler(IEventBus eventBus,
        IVoicePlaybackService voicePlaybackService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.voicePlaybackService = voicePlaybackService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.VoiceParticipantJoined];

    int IMessageHandler.Priority => 11;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not VoiceParticipantJoinedMessage voiceParticipantJoinedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(VoiceCallParticipantJoinedHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceCallParticipantJoinedHandler)} - {message.GetType()}");
        }

        var roomId = context.RoomContext.RoomId;
        var subRoomId = voiceParticipantJoinedMessage.SubRoomId;
        var participant = voiceParticipantJoinedMessage.Participant;

        voicePlaybackService.AddParticipant(participant.Id);

        await eventBus.PublishAsync(new VoiceParticipantJoinedEvent()
        {
            Participant = context.RoomContext.Participants.GetParticipantById(participant.Id),
            SubRoomId = subRoomId,
            RoomId = roomId,
        });

        return HandlerResult.Success();
    }
}
