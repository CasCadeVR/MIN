using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Events;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;

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
        private readonly IParticipantRegistry participantService;
        private CancellationTokenSource? cts;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageReceiver"/>
        /// </summary>
        public MessageReceiver(ITransport transport,
            IMessageSerializer serializer,
            IMessageDispatcher dispatcher,
            IMessageEncryptor encryptor,
            ILoggerProvider logger,
            IParticipantRegistry participantService)
        {
            this.transport = transport;
            this.serializer = serializer;
            this.dispatcher = dispatcher;
            this.encryptor = encryptor;
            this.logger = logger;
            this.participantService = participantService;
        }

        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

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
                participantService.TryGetParticipantInfo(e.ConnectionId, out var participantInfo);

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
                    await dispatcher.DispatchAsync(message, new MessageContext(participantInfo, e.RoomId, e.ConnectionId, cts!.Token));
                }
                catch (Exception ex)
                {
                    logger.Log($"Произошла ошибка во время обработки raw message: {ex.Message}");
                }

                MessageReceived!.Invoke(this, new MessageReceivedEventArgs(e.RoomId, e.ConnectionId, message, participantInfo!));
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время обработки raw message: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            transport.RawMessageReceived -= OnRawMessageReceived;
            cts?.Cancel();
            cts?.Dispose();
            await Task.CompletedTask;
        }
    }
}
