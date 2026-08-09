using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Moderation;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Voice.Handlers;

internal sealed class VoiceCallJoinHandler : IMessageHandler
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly INetworkErrorHandler networkErrorHandler;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="VoiceCallJoinHandler"/>
    /// </summary>
    public VoiceCallJoinHandler(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        INetworkErrorHandler networkErrorHandler,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.networkErrorHandler = networkErrorHandler;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes =>
        [MessageTypeTag.VoiceCallJoinRequest, MessageTypeTag.VoiceCallEstablished];

    int IMessageHandler.Priority => 15;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case VoiceCallJoinRequestMessage voiceCallJoinRequestMessage:
                if (!context.RoomContext.Participants.TryGetParticipantById(message.SenderId, out var sender))
                {
                    return HandlerResult.Failure("Получил сообщение от неизвестного отправителя", stopPropagation: false, critical: true);
                }

                if (context.Role != Role.Host)
                {
                    return HandlerResult.Failure($"Получил сообщение {message.GetType()} в {nameof(VoiceCallJoinHandler)} как {context.Role}, хотя не должен был", stopPropagation: false);
                }

                var roomId = context.RoomContext.RoomId;

                var subRoomInfo = subRoomManager.GetSubRoom(roomId, voiceCallJoinRequestMessage.SubRoomId);

                if (subRoomInfo == null)
                {
                    await networkErrorHandler.SendErrorAsync("Такая подкомната не нашлась", message.SenderId, roomId);
                    return HandlerResult.Success();
                }

                var senderParicipantInfo = sender!.ToParticipantInfo();

                var joinResult = subRoomManager.TryJoinSubRoom(roomId, voiceCallJoinRequestMessage.SubRoomId, senderParicipantInfo);

                if (joinResult != SubRoomJoinOutcome.Success)
                {
                    var error = joinResult switch
                    {
                        SubRoomJoinOutcome.RoomNotFound => "Комната не нашлась",
                        SubRoomJoinOutcome.SubRoomNotFound => "Нету информации о подкомнате",
                        SubRoomJoinOutcome.AlreadyJoined => "Вы уже учавствуете в звонке",
                        SubRoomJoinOutcome.SubRoomNotActive => "Звонок нельзя активировать, только начать новый",
                        _ => "Не удалось войти"
                    };

                    await networkErrorHandler.SendErrorAsync(error, message.SenderId, roomId);
                    return HandlerResult.Success();
                }

                return HandlerResult.WithResponse(new VoiceCallEstablishedMessage()
                {
                    NeedToAnnounce = true,
                    SubRoomId = subRoomInfo.Id,
                });

            case VoiceCallEstablishedMessage voiceCallEstablishedMessage:
                await eventBus.PublishAsync(new VoiceCallEstablishedEvent()
                {
                    RoomId = context.RoomContext.RoomId,
                    SubRoomId = voiceCallEstablishedMessage.SubRoomId,
                });

                if (!voiceCallEstablishedMessage.NeedToAnnounce)
                {
                    return HandlerResult.Success();
                }

                var selfParticipant = identityService.SelfParticipant.ToParticipantInfo();

                var sessionParticipantJoinedMessage = new VoiceParticipantJoinedMessage()
                {
                    SubRoomId = voiceCallEstablishedMessage.SubRoomId,
                    Participant = selfParticipant,
                };

                await messageRouter.RouteAsync(sessionParticipantJoinedMessage, context.RoomContext.RoomId, selfParticipant.Id, context.CancellationToken);

                return HandlerResult.Success();

            default:
                return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(VoiceCallJoinHandler)} - {message.GetType()}");
        }
    }
}
