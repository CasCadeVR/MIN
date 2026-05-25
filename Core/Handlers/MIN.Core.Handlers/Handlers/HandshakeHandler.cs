using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Messaging.Stateless.RoomRelated.Join;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class HandshakeHandler : IMessageHandler, ICoreHandlerAnchor
{
    private readonly IMessageEncryptor encryptor;
    private readonly IGracefulDisconnector gracefulDisconnector;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;
    private readonly IVersionProvider versionProvider;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="HandshakeHandler"/>
    /// </summary>
    public HandshakeHandler(IMessageEncryptor encryptor,
        IGracefulDisconnector gracefulDisconnector,
        IIdentityService identityService,
        IVersionProvider versionProvider,
        ILoggerProvider logger)
    {
        this.encryptor = encryptor;
        this.gracefulDisconnector = gracefulDisconnector;
        this.identityService = identityService;
        this.versionProvider = versionProvider;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes
        => [MessageTypeTag.Handshake, MessageTypeTag.HandshakeAck];

    int IMessageHandler.Priority => 0;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is HandshakeMessage handshakeMessage)
        {
            context.RoomContext.Connections.Register(context.ConnectionId, handshakeMessage.Participant);

            var selfVersion = versionProvider.Version;

            if (!versionProvider.IsVersionCompatible(handshakeMessage.Version))
            {
                await gracefulDisconnector.DisconnectWithReasonAsync(context.ConnectionId,
                    context.RoomContext.RoomId,
                   $"Вы на устаревшей версии: \nВаша версия - {handshakeMessage.Version}\nВерсия хоста комнаты - {selfVersion}");
                return HandlerResult.Success();
            }

            await encryptor.InitializeSessionWithPartnerAsync(handshakeMessage.Participant.Id, handshakeMessage.PublicKey);
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
