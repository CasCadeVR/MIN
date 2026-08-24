using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Exceptions;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless.FastChannelConnect;
using MIN.Core.Messaging.Stateless.Handshake;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Handlers.Handlers;

internal sealed class HandshakeHandler : BaseHandler
{
    private readonly IMessageEncryptor encryptor;
    private readonly IRoomStore roomStore;
    private readonly IIdentityService identityService;
    private readonly IVersionProvider versionProvider;

    public HandshakeHandler(IMessageEncryptor encryptor,
        IRoomStore roomStore,
        IIdentityService identityService,
        IVersionProvider versionProvider,
        ILoggerProvider logger) : base(logger)
    {
        this.encryptor = encryptor;
        this.roomStore = roomStore;
        this.identityService = identityService;
        this.versionProvider = versionProvider;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes
        => [MessageTypeTag.Handshake, MessageTypeTag.HandshakeAck];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        switch (message)
        {
            case HandshakeMessage handshakeMessage:
                if (!context.RoomContext.Connections.TryRegister(context.ConnectionId, handshakeMessage.Participant))
                {
                    return HandlerResult.WithErrorHandled("Произошла коллизия идентификаторов соединения. Попробуйте ещё раз.",
                        critical: true);
                }

                if (versionProvider.IsVersionCompatible(handshakeMessage.Version))
                {
                    var selfVersion = versionProvider.Version;
                    var clientOnOlderVersion = selfVersion > handshakeMessage.Version ? "Вы" : "Хост";
                    return HandlerResult.WithErrorHandled($"{clientOnOlderVersion} на устаревшей версии: \nВаша версия - {handshakeMessage.Version}\nВерсия хоста комнаты - {selfVersion}",
                        critical: true);
                }

                await encryptor.InitializeSessionWithPartnerAsync(handshakeMessage.Participant.Id, handshakeMessage.PublicKey);
                LogInfo($"Сессия с отправителем {handshakeMessage.Participant.Name} инициализирована");

                return HandlerResult.WithResponse(new HandshakeAckMessage()
                {
                    Participant = identityService.SelfParticipant.ToParticipantInfo(),
                    PublicKey = await encryptor.GetLocalPublicKey(),
                });

            case HandshakeAckMessage handshakeAckMessage:
                if (!context.RoomContext.Connections.TryRegister(context.ConnectionId, handshakeAckMessage.Participant))
                {
                    return HandlerResult.Failure("Произошла коллизия идентификаторов соединения с хостом. Попробуйте ещё раз.");
                }

                await encryptor.InitializeSessionWithPartnerAsync(handshakeAckMessage.Participant.Id, handshakeAckMessage.PublicKey);

                LogInfo($"Сессия с получателем {handshakeAckMessage.Participant.Name} инициализирована");

                var savedRoom = roomStore.GetRoom(context.RoomContext.RoomId);

                if (savedRoom == null || !savedRoom.ConnectionAddresses.Any())
                {
                    return HandlerResult.Failure("Не нашлась комната. Попробуйте ещё раз.");
                }

                return HandlerResult.WithResponse(new FastChannelConnectRequestMessage()
                {
                    AddressOrigin = savedRoom.ConnectionAddresses.First().Origin
                });

            default:
                throw new HandlerTypeMismatch(this, message);
        }
    }
}
