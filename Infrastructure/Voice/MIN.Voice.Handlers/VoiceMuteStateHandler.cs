using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceMuteStateHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceMuteStateHandler"/>
    /// </summary>
    public VoiceMuteStateHandler(IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.VoiceMuteState];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not VoiceMuteStateChangedMessage voiceMuteStateChangedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(VoiceMuteStateHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceMuteStateHandler)} - {message.GetType()}");
        }

        await eventBus.PublishAsync(new VoiceMuteStateChangedEvent()
        {
            RoomId = context.RoomContext.RoomId,
            Muted = voiceMuteStateChangedMessage.IsMuted,
            ParticipantId = voiceMuteStateChangedMessage.SenderId,
        });

        return HandlerResult.Success();
    }
}
