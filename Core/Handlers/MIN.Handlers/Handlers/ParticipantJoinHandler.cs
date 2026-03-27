using MIN.Services.Contracts.Interfaces;
using MIN.Events.Contracts;
using MIN.Events.Events;
using MIN.Handlers.Contracts;
using MIN.Handlers.Contracts.Models;
using MIN.Messaging.Contracts;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Messaging.RoomRelated.ParticipantRelated;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ParticipantJoinedMessage"/>
/// </summary>
internal sealed class ParticipantJoinHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IRoomService roomService;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantJoinHandler"/>
    /// </summary>
    public ParticipantJoinHandler(IRoomService roomService, IEventBus eventBus, ILoggerProvider logger)
    {
        this.roomService = roomService;
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
            roomService.AddParticipant(context.RoomId, participantJoinedMessage.Participant);

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
