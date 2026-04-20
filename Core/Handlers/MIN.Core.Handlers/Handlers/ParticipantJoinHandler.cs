using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Messaging.Stateless.RoomRelated;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="RoomJoinRequestMessage"/>,
/// <see cref="RoomJoinResponseMessage"/>,
/// <see cref="ParticipantAcceptedMessage"/>, 
/// <see cref="ParticipantJoinedMessage"/>
/// </summary>
internal sealed class ParticipantJoinHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IRoomStore roomStore;
    private readonly IRoomHoster roomHoster;
    private readonly IIdentityService identityService;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantJoinHandler"/>
    /// </summary>
    public ParticipantJoinHandler(
        IRoomStore roomStore,
        IRoomHoster roomHoster,
        IIdentityService identityService,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.roomStore = roomStore;
        this.roomHoster = roomHoster;
        this.identityService = identityService;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.RoomJoinRequest, MessageTypeTag.RoomJoinResponse,
            MessageTypeTag.ParticipantJoined, MessageTypeTag.ParticipantAccepted];

    int IMessageHandler.Priority => 3;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is RoomJoinRequestMessage roomJoinRequestMessage)
        {
            var response = new RoomJoinResponseMessage()
            {
                RoomId = context.RoomContext.RoomId,
            };
            var room = roomStore.GetRoom(context.RoomContext.RoomId);
            response.Allow = !room.IsFull;
            return HandlerResult.WithResponse(response);
        }
        else if (message is RoomJoinResponseMessage roomJoinResponseMessage)
        {
            if (!roomJoinResponseMessage.Allow)
            {
                return HandlerResult.Failure("Комната уже заполнена", stopPropagation: true);
            }

            var participantJoinedMessage = new ParticipantJoinedMessage()
            {
                Participant = identityService.SelfPartcipant.ToParticipantInfo(),
                RoomId = context.RoomContext.RoomId
            };

            return HandlerResult.WithResponse(participantJoinedMessage);
        }
        else if (message is ParticipantAcceptedMessage participantAcceptedMessage)
        {
            return HandlerResult.WithResponse(new RoomInfoRequestMessage()
            {
                RoomId = context.RoomContext.RoomId,
            });
        }
        else if (message is ParticipantJoinedMessage participantJoinedMessage)
        {
            logger.Log($"Участник {participantJoinedMessage.Participant.Name} зашёл в комнату с id {context.RoomContext.RoomId}");

            context.RoomContext.Participants.AddParticipant(participantJoinedMessage.Participant);
            context.RoomContext.Messages.AddMessage(message);

            await eventBus.PublishAsync(new ParticipantJoinedEvent()
            {
                Message = participantJoinedMessage,
            }, context.CancellationToken);

            if (roomHoster.IsHosting(context.RoomContext.RoomId))
            {
                return HandlerResult.WithResponse(new ParticipantAcceptedMessage()
                {
                    RoomId = context.RoomContext.RoomId,
                });
            }

            return HandlerResult.Success();
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantJoinHandler)} - {message.GetType()}");
    }
}
