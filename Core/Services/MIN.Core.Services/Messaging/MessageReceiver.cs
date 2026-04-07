using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Messaging.Stateless;

namespace MIN.Core.Services.Messaging
{
    /// <inheritdoc cref="IMessageReceiver"/>
    public sealed class MessageReceiver : IMessageReceiver, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IMessageSerializer serializer;
        private readonly IMessageDispatcher dispatcher;
        private readonly IMessageEncryptor encryptor;
        private readonly ILoggerProvider logger;
        private readonly IParticipantConnectionRegistry participantConnectionRegistry;
        private CancellationTokenSource? cts;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageReceiver"/>
        /// </summary>
        public MessageReceiver(ITransport transport,
            IMessageSerializer serializer,
            IMessageDispatcher dispatcher,
            IMessageEncryptor encryptor,
            ILoggerProvider logger,
            IParticipantConnectionRegistry participantConnectionRegistry)
        {
            this.transport = transport;
            this.serializer = serializer;
            this.dispatcher = dispatcher;
            this.encryptor = encryptor;
            this.logger = logger;
            this.participantConnectionRegistry = participantConnectionRegistry;
        }

        async Task IMessageReceiver.StartListeningAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            transport.RawMessageReceived += OnRawMessageReceived;
            await Task.CompletedTask;
        }

        private async void OnRawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
        {
            try
            {
                participantConnectionRegistry.TryGetParticipantFromConnectionId(e.ConnectionId, out var participantInfo);

                byte[] plainData;
                var body = encryptor.RemoveEncryptionHeader(e.Data);

                if (encryptor.IsEncrypted(e.Data))
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
                    if (message is HandshakeAckMessage handshakeAck)
                    {
                        await dispatcher.DispatchAsync(message, new MessageContext(handshakeAck.Participant, e.RoomId, e.ConnectionId, cts!.Token));
                        return;
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

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
        public async ValueTask DisposeAsync()
        {
            transport.RawMessageReceived -= OnRawMessageReceived;
            cts?.Cancel();
            cts?.Dispose();
            await Task.CompletedTask;
        }
    }
}
