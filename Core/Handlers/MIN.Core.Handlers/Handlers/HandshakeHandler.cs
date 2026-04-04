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
using MIN.Core.Services.Contracts.Interfaces.Stores;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;

namespace MIN.Core.Handlers.Handlers;

/// <summary>
/// Обработчик для сообщений <see cref="HandshakeMessage"/>
/// </summary>
internal sealed class HandshakeHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IMessageEncryptor encryptor;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;
    private readonly IParticipantStore participantStore;
    private readonly IMessageSender messageSender;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="HandshakeHandler"/>
    /// </summary>
    public HandshakeHandler(IMessageEncryptor encryptor,
        IParticipantStore participantStore,
        IParticipantConnectionRegistry participantConnectionRegistry,
        IMessageSender messageSender,
        ILoggerProvider logger)
    {
        this.encryptor = encryptor;
        this.participantStore = participantStore;
        this.participantConnectionRegistry = participantConnectionRegistry;
        this.messageSender = messageSender;
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

            var responseHandshake = await encryptor.CreateSelfHandshakeMessageAsync();
            var ackMessage = new HandshakeAckMessage(responseHandshake);

            await messageSender.SendAsync(ackMessage, context.RoomId, responseHandshake.Participant.Id, context.ConnectionId, context.CancellationToken);

            var participantJoinedMessage = new ParticipantJoinedMessage()
            {
                Participant = handshakeMessage.Participant,
                RoomId = context.RoomId
            };

            participantStore.AddParticipant(context.RoomId, handshakeMessage.Participant);
            logger.Log($"Участник {participantJoinedMessage.Participant.Name} зашёл в комнату с id {context.RoomId}");

            await messageSender.BroadcastAsync(participantJoinedMessage, context.RoomId, null, context.CancellationToken);

            return HandlerResult.Success();
        }
        else if (message is HandshakeAckMessage handshakeAckMessage)
        {
            await encryptor.InitializeSessionWithPartnerAsync(handshakeAckMessage.Participant.Id, handshakeAckMessage.PublicKey);
            participantConnectionRegistry.Register(context.ConnectionId, handshakeAckMessage.Participant);

            logger.Log($"Сессия с получателем {handshakeAckMessage.Participant.Name} инициализирована");

            var roomInfoRequest = new RoomInfoRequestMessage(context.RoomId);
            return HandlerResult.WithResponse(roomInfoRequest);
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(HandshakeHandler)} - {message.GetType()}");
    }
}
