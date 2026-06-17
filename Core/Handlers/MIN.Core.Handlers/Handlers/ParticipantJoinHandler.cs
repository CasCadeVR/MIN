using MIN.Core.Entities;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Messaging.Stateless.RoomRelated.Join;
using MIN.Core.Messaging.Stateless.RoomRelated.RoomInfo;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class ParticipantJoinHandler : IMessageHandler
{
    private readonly IRoomStore roomStore;
    private readonly IRoomHoster roomHoster;
    private readonly INetworkErrorHandler networkErrorHandler;
    private readonly IIdentityService identityService;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantJoinHandler"/>
    /// </summary>
    public ParticipantJoinHandler(
        IRoomStore roomStore,
        IRoomHoster roomHoster,
        INetworkErrorHandler networkErrorHandler,
        IIdentityService identityService,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.roomStore = roomStore;
        this.roomHoster = roomHoster;
        this.networkErrorHandler = networkErrorHandler;
        this.identityService = identityService;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.RoomJoinRequest, MessageTypeTag.RoomJoinResponse,
            MessageTypeTag.ParticipantJoined, MessageTypeTag.ParticipantAccepted,
            MessageTypeTag.RoomJoinRejectAck];

    int IMessageHandler.Priority => 3;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case RoomJoinRequestMessage roomJoinRequestMessage:
                if (roomStore.GetRoom(context.RoomContext.RoomId).IsFull)
                {
                    await networkErrorHandler.SendErrorAsync("Комната заполнена",
                        message.SenderId, context.RoomContext.RoomId, critical: true);
                    return HandlerResult.Success();
                }

                return HandlerResult.WithResponse(new RoomJoinResponseMessage());

            case RoomJoinResponseMessage roomJoinResponseMessage:
                return HandlerResult.WithResponse(new ParticipantJoinedMessage()
                {
                    Participant = new Participant(identityService.SelfParticipant)
                });

            case ParticipantAcceptedMessage participantAcceptedMessage:
                return HandlerResult.WithResponse(new RoomInfoRequestMessage());

            case ParticipantJoinedMessage participantJoinedMessage:
                logger.Log($"Участник {participantJoinedMessage.Participant.Name} зашёл в комнату с id {context.RoomContext.RoomId}");

                context.RoomContext.Participants.AddParticipant(new Participant(participantJoinedMessage.Participant));
                context.RoomContext.Messages.AddMessage(message);

                await eventBus.PublishAsync(new ParticipantJoinedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    Message = participantJoinedMessage,
                }, context.CancellationToken);

                if (roomHoster.IsHosting(context.RoomContext.RoomId))
                {
                    return HandlerResult.WithResponse(new ParticipantAcceptedMessage());
                }

                return HandlerResult.Success();

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantJoinHandler)} - {message.GetType()}");
        }
    }
}
