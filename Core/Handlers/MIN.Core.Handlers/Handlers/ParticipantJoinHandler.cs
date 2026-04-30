using System.Collections.Concurrent;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Messaging.Stateless.RoomRelated;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="RoomJoinRequestMessage"/>,
/// <see cref="RoomJoinResponseMessage"/>,
/// <see cref="RoomJoinRejectedAckMessage"/>,
/// <see cref="ParticipantAcceptedMessage"/>, 
/// <see cref="ParticipantJoinedMessage"/>
/// </summary>
internal sealed class ParticipantJoinHandler : IMessageHandler, ICoreHandlerAnchor
{
    private const int TimeoutUponReceivingRejectionAck = 5000;

    private readonly IRoomStore roomStore;
    private readonly ITransport transport;
    private readonly IRoomHoster roomHoster;
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, Timer> rejectAckTimers = new();

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="ParticipantJoinHandler"/>
    /// </summary>
    public ParticipantJoinHandler(
        IRoomStore roomStore,
        ITransport transport,
        IRoomHoster roomHoster,
        IMessageRouter messageRouter,
        IIdentityService identityService,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.roomStore = roomStore;
        this.transport = transport;
        this.roomHoster = roomHoster;
        this.messageRouter = messageRouter;
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
                var response = new RoomJoinResponseMessage()
                {
                    RoomId = context.RoomContext.RoomId,
                };
                var room = roomStore.GetRoom(context.RoomContext.RoomId);
                response.Allow = !room.IsFull;
                response.Reason = response.Allow ? null : "Комната заполнена";

                if (!response.Allow)
                {
                    StartRejectAckTimer(context, response.Id, response.Reason ?? "Вход был запрещён");
                }

                return HandlerResult.WithResponse(response);

            case RoomJoinResponseMessage roomJoinResponseMessage:
                if (!roomJoinResponseMessage.Allow)
                {
                    var reason = roomJoinResponseMessage.Reason ?? "Вход был запрещён";
                    await messageRouter.RouteAsync(new RoomJoinRejectedAckMessage()
                    {
                        RejectionMessageId = roomJoinResponseMessage.Id,
                        Reason = reason,
                    }, context.RoomContext.RoomId, identityService.SelfParticipant.Id, context.CancellationToken);
                    return HandlerResult.Failure(reason, stopPropagation: true);
                }

                var selfparticipantJoinedMessage = new ParticipantJoinedMessage()
                {
                    Participant = identityService.SelfParticipant.ToParticipantInfo(),
                    RoomId = context.RoomContext.RoomId
                };

                return HandlerResult.WithResponse(selfparticipantJoinedMessage);

            case RoomJoinRejectedAckMessage rejectedAckMessage:
                ResetRejectAckTimer(rejectedAckMessage.RejectionMessageId);
                await transport.DisconnectClientAsync(context.RoomContext.RoomId, context.ConnectionId, rejectedAckMessage.Reason);
                return HandlerResult.Success();

            case ParticipantAcceptedMessage participantAcceptedMessage:
                return HandlerResult.WithResponse(new RoomInfoRequestMessage()
                {
                    RoomId = context.RoomContext.RoomId,
                });

            case ParticipantJoinedMessage participantJoinedMessage:
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

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(ParticipantJoinHandler)} - {message.GetType()}");
        }
    }

    private void StartRejectAckTimer(MessageContext context, Guid rejectedAckMessageId, string reason)
    {
        var timer = new Timer(
            OnRejectAckTimeout,
            (context, rejectedAckMessageId, reason),
            DateTime.UtcNow.AddMilliseconds(TimeoutUponReceivingRejectionAck) - DateTime.UtcNow,
            Timeout.InfiniteTimeSpan);

        rejectAckTimers.TryAdd(rejectedAckMessageId, timer);
    }

    private async void OnRejectAckTimeout(object? state)
    {
        if (state is (MessageContext context, Guid rejectionMessageIdstream, string reason))
        {
            await transport.DisconnectClientAsync(context.RoomContext.RoomId, context.ConnectionId, reason);
            ResetRejectAckTimer(rejectionMessageIdstream);
        }
    }

    private void ResetRejectAckTimer(Guid rejectionMessageIdstream)
    {
        if (rejectAckTimers.TryGetValue(rejectionMessageIdstream, out var existingTimer))
        {
            existingTimer.Dispose();
        }
    }
}
