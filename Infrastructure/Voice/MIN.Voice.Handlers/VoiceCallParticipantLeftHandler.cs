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

internal sealed class VoiceCallParticipantLeftHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly IVoicePlaybackService voicePlaybackService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallParticipantLeftHandler"/>
    /// </summary>
    public VoiceCallParticipantLeftHandler(IEventBus eventBus,
        IVoicePlaybackService voicePlaybackService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.voicePlaybackService = voicePlaybackService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.VoiceParticipantLeft];

    int IMessageHandler.Priority => 11;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not VoiceParticipantLeftMessage voiceParticipantLeftMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(VoiceCallParticipantLeftHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceCallParticipantLeftHandler)} - {message.GetType()}");
        }

        var roomId = context.RoomContext.RoomId;
        var subRoomId = voiceParticipantLeftMessage.SubRoomId;
        var participant = voiceParticipantLeftMessage.Participant;

        if (voicePlaybackService.IsInVoiceCall(subRoomId))
        {
            voicePlaybackService.RemoveParticipant(participant.Id);
        }

        await eventBus.PublishAsync(new VoiceParticipantLeftEvent()
        {
            Participant = participant,
            SubRoomId = subRoomId,
            RoomId = roomId,
        });

        return HandlerResult.Success();
    }
}
