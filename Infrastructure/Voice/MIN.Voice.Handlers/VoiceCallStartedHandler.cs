using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallStartedHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallStartedHandler"/>
    /// </summary>
    public VoiceCallStartedHandler(IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.VoiceCallStarted];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not VoiceCallStartedMessage voiceCallStartedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(VoiceCallStartedHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceCallStartedHandler)} - {message.GetType()}");
        }

        context.RoomContext.Messages.AddMessage(voiceCallStartedMessage);

        await eventBus.PublishAsync(new VoiceCallStartedEvent()
        {
            Message = voiceCallStartedMessage,
            RoomId = context.RoomContext.RoomId,
            Participant = context.RoomContext.Participants.GetParticipantById(voiceCallStartedMessage.Sender.Id)
        });

        return HandlerResult.Success();
    }
}
