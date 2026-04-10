using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Services.Contracts.Interfaces.Stores;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ParticipantJoinedMessage"/>
/// </summary>
internal sealed class ParticipantJoinHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IParticipantStore participantStore;
    private readonly IMessageStore messageStore;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantJoinHandler"/>
    /// </summary>
    public ParticipantJoinHandler(IParticipantStore participantStore,
        IMessageStore messageStore,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.participantStore = participantStore;
        this.messageStore = messageStore;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.ParticipantJoined];

    int IMessageHandler.Priority => 3;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is ParticipantJoinedMessage participantJoinedMessage)
        {
            participantStore.AddParticipant(context.RoomId, participantJoinedMessage.Participant);
            logger.Log($"Участник {participantJoinedMessage.Participant.Name} зашёл в комнату с id {context.RoomId}");
            messageStore.AddMessage(context.RoomId, message);

            await eventBus.PublishAsync(new ParticipantJoinedEvent()
            {
                Message = participantJoinedMessage,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantJoinHandler)} - {message.GetType()}");
    }
}
