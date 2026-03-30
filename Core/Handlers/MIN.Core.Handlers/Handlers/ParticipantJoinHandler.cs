using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Services.Contracts.Interfaces.Rooms;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ParticipantJoinedMessage"/>
/// </summary>
internal sealed class ParticipantJoinHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IRoomRegistry roomRegistry;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantJoinHandler"/>
    /// </summary>
    public ParticipantJoinHandler(IRoomRegistry roomRegistry, IEventBus eventBus, ILoggerProvider logger)
    {
        this.roomRegistry = roomRegistry;
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
            roomRegistry.AddParticipant(context.RoomId, participantJoinedMessage.Participant);

            logger.Log($"Участник {participantJoinedMessage.Participant.Name} зашёл в комнату");

            await eventBus.PublishAsync(new ParticipantJoinedEvent()
            {
                Participant = participantJoinedMessage.Participant,
                RoomId = participantJoinedMessage.RoomId,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantJoinHandler)} - {message.GetType()}");
    }
}
