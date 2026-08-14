using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.FastChannelConnect;
using MIN.Core.Messaging.Stateless.Handshake;
using MIN.Core.Services.Contracts.Interfaces.Moderation;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class HandshakeHandler : IMessageHandler
{
    private readonly IMessageEncryptor encryptor;
    private readonly INetworkErrorHandler networkErrorHandler;
    private readonly IRoomStore roomStore;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;
    private readonly IVersionProvider versionProvider;

    public HandshakeHandler(IMessageEncryptor encryptor,
        INetworkErrorHandler networkErrorHandler,
        IRoomStore roomStore,
        IIdentityService identityService,
        IVersionProvider versionProvider,
        ILoggerProvider logger)
    {
        this.encryptor = encryptor;
        this.networkErrorHandler = networkErrorHandler;
        this.roomStore = roomStore;
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
            var selfVersion = versionProvider.Version;

            if (!versionProvider.IsVersionCompatible(handshakeMessage.Version))
            {
                var clientOnOlderVersion = selfVersion > handshakeMessage.Version ? "Вы" : "Хост";
                await networkErrorHandler.SendErrorAsync(
                    $"{clientOnOlderVersion} на устаревшей версии: \nВаша версия - {handshakeMessage.Version}\nВерсия хоста комнаты - {selfVersion}",
                    handshakeMessage.Participant.Id,
                    context.RoomContext.RoomId,
                    critical: true);
                return HandlerResult.Success();
            }

            if (!context.RoomContext.Connections.TryRegister(context.ConnectionId, handshakeMessage.Participant))
            {
                await networkErrorHandler.SendErrorAsync(
                    "Произошла коллизия идентификаторов соединения. Попробуйте ещё раз.",
                    handshakeMessage.Participant.Id,
                    context.RoomContext.RoomId,
                    critical: true);
                return HandlerResult.Success();
            }

            await encryptor.InitializeSessionWithPartnerAsync(handshakeMessage.Participant.Id, handshakeMessage.PublicKey);
            logger.Log($"Сессия с отправителем {handshakeMessage.Participant.Name} инициализирована");

            return HandlerResult.WithResponse(new HandshakeAckMessage()
            {
                Participant = identityService.SelfParticipant.ToParticipantInfo(),
                PublicKey = await encryptor.GetLocalPublicKey(),
            });
        }
        else if (message is HandshakeAckMessage handshakeAckMessage)
        {
            if (!context.RoomContext.Connections.TryRegister(context.ConnectionId, handshakeAckMessage.Participant))
            {
                return HandlerResult.Failure("Произошла коллизия идентификаторов соединения с хостом. Попробуйте ещё раз.");
            }

            await encryptor.InitializeSessionWithPartnerAsync(handshakeAckMessage.Participant.Id, handshakeAckMessage.PublicKey);

            logger.Log($"Сессия с получателем {handshakeAckMessage.Participant.Name} инициализирована");

            var savedRoom = roomStore.GetRoom(context.RoomContext.RoomId);

            if (savedRoom == null || !savedRoom.ConnectionAddresses.Any())
            {
                return HandlerResult.Failure("Не нашлась комната. Попробуйте ещё раз.");
            }

            return HandlerResult.WithResponse(new FastChannelConnectRequestMessage()
            {
                AddressOrigin = savedRoom.ConnectionAddresses.First().Origin
            });
        }

        return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(HandshakeHandler)} - {message.GetType()}");
    }
}
