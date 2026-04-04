using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ParticipantLeftMessage"/>
/// </summary>
internal sealed class ParticipantLeftHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IMessageStore messageStore;
    private readonly IParticipantStore participantStore;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantLeftHandler"/>
    /// </summary>
    public ParticipantLeftHandler(IMessageStore messageStore,
        IParticipantStore participantStore,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.messageStore = messageStore;
        this.participantStore = participantStore;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.ParticipantJoined];

    int IMessageHandler.Priority => 3;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is ParticipantLeftMessage participantLeftMessage)
        {
            messageStore.AddMessage(context.RoomId, message);

            participantStore.RemoveParticipant(context.RoomId, participantLeftMessage.Participant.Id);

            logger.Log($"Участник {participantLeftMessage.Participant.Name} вышел из комнаты");

            await eventBus.PublishAsync(new ParticipantLeftEvent()
            {
                Message = participantLeftMessage,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantLeftHandler)} - {message.GetType()}");
    }
}
