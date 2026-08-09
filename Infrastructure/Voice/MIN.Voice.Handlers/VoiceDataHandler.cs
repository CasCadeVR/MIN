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

internal sealed class VoiceDataHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly IVoicePlaybackService voicePlaybackService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceDataHandler"/>
    /// </summary>
    public VoiceDataHandler(IEventBus eventBus,
        IVoicePlaybackService voicePlaybackService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.voicePlaybackService = voicePlaybackService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.VoiceData];

    int IMessageHandler.Priority => 1;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not VoiceDataMessage voiceData)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(VoiceDataHandler)} - {message.GetType()}");
            return HandlerResult.Success();
        }

        voicePlaybackService.PlaySamples(message.SenderId, voiceData.SequenceNumber, voiceData.Data);

        await eventBus.PublishAsync(new VoiceDataReceivedEvent()
        {
            RoomId = context.RoomContext.RoomId,
            ParticipantId = message.SenderId
        });

        return HandlerResult.Success();
    }
}
