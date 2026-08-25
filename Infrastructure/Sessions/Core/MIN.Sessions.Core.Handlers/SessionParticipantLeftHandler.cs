using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.Ipc;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.Handlers;

internal sealed class SessionParticipantLeftHandler : BaseHandler
{
    private readonly ISessionProcessBridge sessionProcessBridge;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="SessionParticipantLeftHandler"/>
    /// </summary>
    public SessionParticipantLeftHandler(ISessionProcessBridge sessionProcessBridge,
        ILoggerProvider logger) : base(logger)
    {
        this.sessionProcessBridge = sessionProcessBridge;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.SessionParticipantLeft];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var sessionParticipantLeftMessage = (SessionParticipantLeftMessage)message;
        var roomId = context.RoomContext.RoomId;
        var subRoomId = sessionParticipantLeftMessage.SubRoomId;
        var participant = sessionParticipantLeftMessage.Participant;

        if (!sessionParticipantLeftMessage.IsLast)
        {
            var processContexts = sessionProcessBridge.GetConnections(roomId, subRoomId);

            foreach (var processContext in processContexts)
            {
                await sessionProcessBridge.SendIpcMessage(new ParticipantDisconnectedMessage(participant.Id.ToString()),
                    processContext, message.SenderId, context.CancellationToken);
            }
        }

        var existingSessionReadyMessageId = context.RoomContext.Messages.GetHistory()
            .OfType<SessionReadyMessage>().FirstOrDefault(x => x.SubRoomId == sessionParticipantLeftMessage.SubRoomId)?.Id;

        if (existingSessionReadyMessageId != null)
        {
            var existing = context.RoomContext.Messages.GetMessageById(existingSessionReadyMessageId.Value) as SessionReadyMessage;
            existing!.CurrentParticipantAmount--;
            context.RoomContext.Messages.UpdateMessage(existing.Id, existing);
        }

        return HandlerResult.WithEvent(new SessionParticipantLeftEvent()
        {
            Participant = participant,
            SubRoomId = subRoomId,
            RoomId = roomId,
        });
    }
}
