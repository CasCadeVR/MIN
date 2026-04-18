using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Messaging.Stateless.RoomRelated;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="ParticipantJoinedMessage"/>, <see cref="ParticipantJoinedMessage"/>, <see cref="ParticipantJoinedMessage"/>,
/// </summary>
internal sealed class ParticipantJoinHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IRoomStore roomStore;
    private readonly IParticipantStore participantStore;
    private readonly IIdentityService identityService;
    private readonly IMessageRouter messageRouter;
    private readonly IMessageStore messageStore;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantJoinHandler"/>
    /// </summary>
    public ParticipantJoinHandler(
        IRoomStore roomStore,
        IParticipantStore participantStore,
        IIdentityService identityService,
        IMessageRouter messageRouter,
        IMessageStore messageStore,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.roomStore = roomStore;
        this.participantStore = participantStore;
        this.identityService = identityService;
        this.messageRouter = messageRouter;
        this.messageStore = messageStore;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.RoomJoinRequest, MessageTypeTag.RoomJoinResponse, MessageTypeTag.ParticipantJoined];

    int IMessageHandler.Priority => 3;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is RoomJoinRequestMessage roomJoinRequestMessage)
        {
            var response = new RoomJoinResponseMessage()
            {
                RoomId = context.RoomId,
            };
            var room = roomStore.GetRoom(context.RoomId);
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
                RoomId = context.RoomId
            };

            await messageRouter.RouteAsync(participantJoinedMessage,
                context.RoomId,
                identityService.SelfPartcipant.Id,
                Recipient.FromConnection(context.RoomId, context.ConnectionId),
                context.CancellationToken);

            return HandlerResult.WithResponse(new RoomInfoRequestMessage()
            {
                RoomId = context.RoomId,
            });
        }
        else if (message is ParticipantJoinedMessage participantJoinedMessage)
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
