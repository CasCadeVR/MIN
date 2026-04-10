using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Messaging.Stateless.RoomRelated;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Models;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="HandshakeMessage"/>
/// </summary>
internal sealed class HandshakeHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IMessageEncryptor encryptor;
    private readonly IIdentityService identityService;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;
    private readonly IMessageSender messageSender;
    private readonly IMessageRouter messageRouter;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="HandshakeHandler"/>
    /// </summary>
    public HandshakeHandler(IMessageEncryptor encryptor,
        IIdentityService identityService,
        IParticipantConnectionRegistry participantConnectionRegistry,
        IMessageSender messageSender,
        IMessageRouter messageRouter,
        ILoggerProvider logger)
    {
        this.encryptor = encryptor;
        this.identityService = identityService;
        this.participantConnectionRegistry = participantConnectionRegistry;
        this.messageSender = messageSender;
        this.messageRouter = messageRouter;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.Handshake, MessageTypeTag.HandshakeAck];

    int IMessageHandler.Priority => 0;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is HandshakeMessage handshakeMessage)
        {
            await encryptor.InitializeSessionWithPartnerAsync(handshakeMessage.Participant.Id, handshakeMessage.PublicKey);
            participantConnectionRegistry.Register(context.ConnectionId, handshakeMessage.Participant);
            logger.Log($"Сессия с отправителем {handshakeMessage.Participant.Name} инициализирована");

            var ackMessage = new HandshakeAckMessage()
            {
                Participant = new ParticipantInfo(identityService.SelfPartcipant),
                PublicKey = await encryptor.GetLocalPublicKey(),
            };

            await messageSender.SendAsync(ackMessage, context.RoomId, handshakeMessage.Participant.Id, context.ConnectionId, context.CancellationToken);

            return HandlerResult.Success();
        }
        else if (message is HandshakeAckMessage handshakeAckMessage)
        {
            await encryptor.InitializeSessionWithPartnerAsync(handshakeAckMessage.Participant.Id, handshakeAckMessage.PublicKey);
            participantConnectionRegistry.Register(context.ConnectionId, handshakeAckMessage.Participant);

            logger.Log($"Сессия с получателем {handshakeAckMessage.Participant.Name} инициализирована");

            var participantJoinedMessage = new ParticipantJoinedMessage()
            {
                Participant = new ParticipantInfo(identityService.SelfPartcipant),
                RoomId = context.RoomId
            };

            await messageRouter.RouteAsync(participantJoinedMessage,
                context.RoomId,
                identityService.SelfPartcipant.Id,
                Recipient.FromConnection(context.ConnectionId),
                context.CancellationToken);

            var roomInfoRequest = new RoomInfoRequestMessage(context.RoomId);
            return HandlerResult.WithResponse(roomInfoRequest);
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(HandshakeHandler)} - {message.GetType()}");
    }
}
