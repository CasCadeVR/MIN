using System.Collections.Concurrent;
using MIN.Services.Contracts.Interfaces.Messaging;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Entities.Contracts.Models;
using MIN.Messaging.Contracts.Events;
using MIN.Serialization.Contracts;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Transport.Contracts.Events;
using MIN.Transport.Contracts.Interfaces;

namespace MIN.Services.Messaging
{
    /// <inheritdoc cref="IMessageReceiver"/>
    internal sealed class MessageReceiver : IMessageReceiver, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IMessageSerializer serializer;
        private readonly IMessageEncryptor encryptor;
        private readonly ILoggerProvider logger;
        private readonly ConcurrentDictionary<Guid, ParticipantInfo> participants = new();
        private CancellationTokenSource? cts;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageReceiver"/>
        /// </summary>
        public MessageReceiver(ITransport transport,
            IMessageSerializer serializer,
            IMessageEncryptor encryptor,
            ILoggerProvider logger,
            ConcurrentDictionary<Guid, ParticipantInfo> participants)
        {
            this.transport = transport;
            this.serializer = serializer;
            this.encryptor = encryptor;
            this.logger = logger;
            this.participants = participants;
        }

        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        async Task IMessageReceiver.StartListeningAsync(CancellationToken cancellationToken = default)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            transport.RawMessageReceived += OnRawMessageReceived;
            await Task.CompletedTask;
        }

        private async void OnRawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
        {
            try
            {
                participants.TryGetValue(e.ConnectionId, out var participantInfo);

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
