using MIN.Services.Contracts.Interfaces;
using MIN.Events.Contracts;
using MIN.Events.Events;
using MIN.Handlers.Contracts;
using MIN.Handlers.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ParticipantLeftMessage"/>
/// </summary>
internal sealed class ParticipantLeftHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IRoomService roomService;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantLeftHandler"/>
    /// </summary>
    public ParticipantLeftHandler(IRoomService roomService, IEventBus eventBus, ILoggerProvider logger)
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
        if (message is ParticipantLeftMessage participantLeftMessage)
        {
            roomService.RemoveParticipant(context.RoomId, participantLeftMessage.Participant.Id);

            logger.Log($"Участник {participantLeftMessage.Participant.Name} вышел из комнаты");

            await eventBus.PublishAsync(new ParticipantLeftEvent()
            {
                Participant = participantLeftMessage.Participant,
                RoomId = participantLeftMessage.RoomId,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantLeftHandler)} - {message.GetType()}");
    }
}
