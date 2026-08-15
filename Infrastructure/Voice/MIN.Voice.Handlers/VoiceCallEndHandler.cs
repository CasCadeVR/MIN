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

internal sealed class VoiceCallEndHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly IVoicePlaybackService voicePlaybackService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallEndHandler"/>
    /// </summary>
    public VoiceCallEndHandler(IEventBus eventBus,
        IVoicePlaybackService voicePlaybackService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.voicePlaybackService = voicePlaybackService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.VoiceCallEnded];

    int IMessageHandler.Priority => 8;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not VoiceCallEndedMessage voiceCallEndedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(VoiceCallEndHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceCallEndHandler)} - {message.GetType()}");
        }

        var existingVoiceCallStartedMessageId = context.RoomContext.Messages.GetHistory()
            .OfType<VoiceCallStartedMessage>().FirstOrDefault(x => x.SubRoomId == voiceCallEndedMessage.SubRoomId)?.Id;

        if (existingVoiceCallStartedMessageId != null)
        {
            var existing = context.RoomContext.Messages.GetMessageById(existingVoiceCallStartedMessageId.Value) as VoiceCallStartedMessage;
            existing!.EndedAt = DateTime.Now;
            context.RoomContext.Messages.UpdateMessage(existing.Id, existing);
        }

        if (voicePlaybackService.IsInVoiceCall(voiceCallEndedMessage.SubRoomId))
        {
            await eventBus.PublishAsync(new VoiceCallLeftEvent()
            {
                RoomId = context.RoomContext.RoomId,
                SubRoomId = voiceCallEndedMessage.SubRoomId
            });
        }

        voicePlaybackService.Clear();

        await eventBus.PublishAsync(new VoiceCallEndedEvent()
        {
            RoomId = context.RoomContext.RoomId,
            SubRoomId = voiceCallEndedMessage.SubRoomId
        });

        return HandlerResult.Success();
    }
}
