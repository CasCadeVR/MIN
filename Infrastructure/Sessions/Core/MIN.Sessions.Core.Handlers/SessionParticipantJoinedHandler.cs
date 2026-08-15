using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.Ipc;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionParticipantJoinedHandler : IMessageHandler
{
    private readonly IEventBus eventBus;
    private readonly ISessionProcessBridge sessionProcessBridge;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionParticipantJoinedHandler"/>
    /// </summary>
    public SessionParticipantJoinedHandler(IEventBus eventBus,
        ISessionProcessBridge sessionProcessBridge,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.sessionProcessBridge = sessionProcessBridge;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.SessionParticipantJoined];

    int IMessageHandler.Priority => 11;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not SessionParticipantJoinedMessage sessionParticipantJoinedMessage)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(SessionParticipantJoinedHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(SessionParticipantJoinedHandler)} - {message.GetType()}");
        }

        var roomId = context.RoomContext.RoomId;
        var subRoomId = sessionParticipantJoinedMessage.SubRoomId;
        var participant = sessionParticipantJoinedMessage.Participant;

        var processContexts = sessionProcessBridge.GetConnections(roomId, subRoomId);

        foreach (var processContext in processContexts)
        {
            await sessionProcessBridge.SendIpcMessage(new ParticipantConnectedMessage(participant.Id.ToString(),
                participant.Name), processContext, message.SenderId, context.CancellationToken);
        }

        var existingSessionReadyMessageId = context.RoomContext.Messages.GetHistory()
            .OfType<SessionReadyMessage>().FirstOrDefault(x => x.SubRoomId == sessionParticipantJoinedMessage.SubRoomId)?.Id;

        if (existingSessionReadyMessageId != null)
        {
            var existing = context.RoomContext.Messages.GetMessageById(existingSessionReadyMessageId.Value) as SessionReadyMessage;
            existing!.CurrentParticipantAmount++;
            context.RoomContext.Messages.UpdateMessage(existing.Id, existing);
        }

        await eventBus.PublishAsync(new SessionParticipantJoinedEvent()
        {
            Participant = participant,
            SubRoomId = subRoomId,
            RoomId = roomId,
        });

        return HandlerResult.Success();
    }
}
