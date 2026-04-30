using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Messaging.Stateless.RoomRelated;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="HandshakeMessage"/>
/// </summary>
internal sealed class HandshakeHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IMessageEncryptor encryptor;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="HandshakeHandler"/>
    /// </summary>
    public HandshakeHandler(IMessageEncryptor encryptor,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.encryptor = encryptor;
        this.identityService = identityService;
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
            context.RoomContext.Connections.Register(context.ConnectionId, handshakeMessage.Participant);
            logger.Log($"Сессия с отправителем {handshakeMessage.Participant.Name} инициализирована");

            var ackMessage = new HandshakeAckMessage()
            {
                Participant = identityService.SelfParticipant.ToParticipantInfo(),
                PublicKey = await encryptor.GetLocalPublicKey(),
            };

            return HandlerResult.WithResponse(ackMessage);
        }
        else if (message is HandshakeAckMessage handshakeAckMessage)
        {
            await encryptor.InitializeSessionWithPartnerAsync(handshakeAckMessage.Participant.Id, handshakeAckMessage.PublicKey);
            context.RoomContext.Connections.Register(context.ConnectionId, handshakeAckMessage.Participant);

            logger.Log($"Сессия с получателем {handshakeAckMessage.Participant.Name} инициализирована");

            return HandlerResult.WithResponse(new RoomJoinRequestMessage()
            {
                RoomId = context.RoomContext.RoomId
            });
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(HandshakeHandler)} - {message.GetType()}");
    }
}
