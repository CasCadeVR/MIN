using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Constants;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Messaging
{
    /// <inheritdoc cref="IMessageReceiver"/>
    public sealed class MessageReceiver : IMessageReceiver, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IMessageSerializer serializer;
        private readonly IEventBus eventBus;
        private readonly IMessageDispatcher dispatcher;
        private readonly IIdentityService identityService;
        private readonly IMessageEncryptor encryptor;
        private readonly ILoggerProvider logger;
        private readonly IParticipantConnectionRegistry participantConnectionRegistry;
        private CancellationTokenSource? cts;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageReceiver"/>
        /// </summary>
        public MessageReceiver(ITransport transport,
            IMessageSerializer serializer,
            IEventBus eventBus,
            IMessageDispatcher dispatcher,
            IIdentityService identityService,
            IMessageEncryptor encryptor,
            ILoggerProvider logger,
            IParticipantConnectionRegistry participantConnectionRegistry)
        {
            this.transport = transport;
            this.serializer = serializer;
            this.eventBus = eventBus;
            this.dispatcher = dispatcher;
            this.identityService = identityService;
            this.encryptor = encryptor;
            this.logger = logger;
            this.participantConnectionRegistry = participantConnectionRegistry;
        }

        async Task IMessageReceiver.StartListeningAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            transport.RawMessageReceived += OnRawMessageReceived;
            eventBus.Subscribe<LocalMessageRecievedEvent>(OnLocalMessageRecieved);
            await Task.CompletedTask;
        }

        private async Task OnLocalMessageRecieved(LocalMessageRecievedEvent e, CancellationToken ct)
        {
            await dispatcher.DispatchAsync(e.Message, new MessageContext(new ParticipantInfo(identityService.SelfPartcipant), e.RoomId, Guid.Empty, ct));
        }

        private async void OnRawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
        {
            try
            {
                participantConnectionRegistry.TryGetParticipantFromConnectionId(e.ConnectionId, out var participantInfo);

                byte[] plainData;
                var body = encryptor.RemoveEncryptionHeader(e.Data);

                if (encryptor.IsEncrypted(e.Data) && e.ConnectionId != CoreServicesConstants.LocalConnectionId)
                {
                    if (participantInfo == null)
                    {
                        logger.Log($"Получили зашифрованное сообщение от неизвестного соединения с id {e.ConnectionId}, игнорю");
                        return;
                    }
                    plainData = encryptor.DecryptMessage(body, participantInfo.Id);
                }
                else
                {
                    plainData = body;
                }

                var message = serializer.Deserialize(plainData);

                try
                {
                    if (message is HandshakeAckMessage ackMessage)
                    {
                        participantInfo = ackMessage.Participant;
                    }

                    await dispatcher.DispatchAsync(message, new MessageContext(participantInfo, e.RoomId, e.ConnectionId, cts!.Token));
                }
                catch (Exception ex)
                {
                    logger.Log($"Произошла ошибка во время обработки raw message: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время обработки raw message: {ex.Message}");
            }
        }

        /// <summary>
        /// Закончить прослушку сообщений
        /// </summary>
        public async Task StopListeningAsync()
        {
            transport.RawMessageReceived -= OnRawMessageReceived;
            cts?.Cancel();
            cts?.Dispose();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Обработать сообщение вручную
        /// </summary>
        async Task IMessageReceiver.ReceiveAsLocal(IMessage message, ParticipantInfo sender, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
        {
            await dispatcher.DispatchAsync(message, new MessageContext(sender, roomId, connectionId, cancellationToken));
        }

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
        public async ValueTask DisposeAsync() => await StopListeningAsync();
    }
}
