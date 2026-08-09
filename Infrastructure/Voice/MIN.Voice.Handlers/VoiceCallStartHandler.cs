using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallStartHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageSender messageSender;
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallStartHandler"/>
    /// </summary>
    public VoiceCallStartHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageSender messageSender,
        IMessageRouter messageRouter,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageSender = messageSender;
        this.messageRouter = messageRouter;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.VoiceCallStartRequest];

    int IMessageHandler.Priority => 12;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not VoiceCallStartRequestMessage _)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(VoiceCallStartHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceCallStartHandler)} - {message.GetType()}");
        }

        if (context.Role != Role.Host)
        {
            return HandlerResult.Failure($"Получил сообщение {message.GetType()} в {nameof(VoiceCallStartHandler)} как {context.Role}, хотя не должен был", stopPropagation: false);
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
        }

        var senderParicipantInfo = sender!.ToParticipantInfo();
        var subRoomInfo = subRoomManager.HostSubRoom(context.RoomContext.RoomId, senderParicipantInfo, SubRoomPurpose.Voice);
        var subRoomId = subRoomInfo.Id;

        var voiceCallStartedMessage = new VoiceCallStartedMessage()
        {
            SubRoomId = subRoomId,
            Sender = senderParicipantInfo,
            SenderId = message.SenderId,
        };

        await messageRouter.RouteAsync(voiceCallStartedMessage, context.RoomContext.RoomId, message.SenderId, context.CancellationToken);

        if (identityService.SelfParticipant.Id == message.SenderId)
        {
            await eventBus.PublishAsync(new VoiceCallEstablishedEvent()
            {
                RoomId = context.RoomContext.RoomId,
                SubRoomId = subRoomId,
            }, context.CancellationToken);

            return HandlerResult.Success();
        }
        else
        {
            // sending him ready as he didnt received by sender filtering
            await messageSender.SendAsync(voiceCallStartedMessage, context.RoomContext.RoomId, context.ConnectionId, context.CancellationToken);

            return HandlerResult.WithResponse(new VoiceCallEstablishedMessage()
            {
                NeedToAnnounce = false,
                SubRoomId = subRoomId,
            });
        }
    }
}
