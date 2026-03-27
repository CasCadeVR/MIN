using MIN.Application.Contracts.Interfaces.Messaging;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Events.Events;
using MIN.Handlers.Contracts;
using MIN.Handlers.Contracts.Models;
using MIN.Messaging.Contracts;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Messaging.RoomRelated.ParticipantRelated;
using MIN.Messaging.Stateless;
using MIN.Messaging.Stateless.RoomRelated;
using MIN.Services.Contracts.Interfaces;

namespace MIN.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="HandshakeMessage"/>
/// </summary>
internal sealed class HandshakeHandler : IMessageHandler, IHandlerAnchor
{
    private readonly IMessageEncryptor messageEncryptor;
    private readonly IMessageService messageService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="HandshakeHandler"/>
    /// </summary>
    public HandshakeHandler(IMessageEncryptor messageEncryptor, IMessageService messageService, ILoggerProvider logger)
    {
        this.messageEncryptor = messageEncryptor;
        this.messageService = messageService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.Handshake, MessageTypeTag.HandshakeAck];

    int IMessageHandler.Priority => 0;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is HandshakeMessage handshakeMessage)
        {
            await messageEncryptor.InitializeSessionWithPartnerAsync(handshakeMessage.Participant.Id, handshakeMessage.PublicKey);
            messageService.SetParticipantInfo(context.ConnectionId, handshakeMessage.Participant);
            logger.Log($"Сессия с отправителем {handshakeMessage.Participant.Name} инициализирована");

            var responseHandshake = await messageEncryptor.CreateSelfHandshakeMessageAsync();
            var ackMessage = new HandshakeAckMessage(responseHandshake);

            await messageService.SendAsync(ackMessage, context.RoomId, context.ConnectionId, context.CancellationToken);

            var participantJoinedMessage = new ParticipantJoinedMessage()
            {
                Participant = handshakeMessage.Participant,
                RoomId = context.RoomId
            };

            await messageService.BroadcastAsync(participantJoinedMessage, context.RoomId, null, context.CancellationToken);

            return HandlerResult.Success();
        }
        else if (message is HandshakeAckMessage handshakeAckMessage)
        {
            await messageEncryptor.InitializeSessionWithPartnerAsync(handshakeAckMessage.Participant.Id, handshakeAckMessage.PublicKey);
            messageService.SetParticipantInfo(context.ConnectionId, handshakeAckMessage.Participant);

            logger.Log($"Сессия с получателем {handshakeAckMessage.Participant.Name} инициализирована");

            var roomInfoRequest = new RoomInfoRequestMessage(context.RoomId);
            return HandlerResult.WithResponse(roomInfoRequest);
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(HandshakeHandler)} - {message.GetType()}");
    }
}
