using System.Collections.Concurrent;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Contracts.Events;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Serialization.Contracts;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Events;
using MIN.Transport.Contracts.Interfaces;

namespace MIN.Application
{
    /// <inheritdoc cref="MessageService"/>
    public sealed class MessageService : IMessageService, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IMessageSerializer serializer;
        private readonly IMessageEncryptor encryptor;
        private readonly ILoggerProvider logger;
        private readonly ConcurrentDictionary<Guid, ParticipantInfo> participants = new();
        private CancellationTokenSource? cts;
        private bool disposed;

        /// <inheritdoc />
        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageService"/>
        /// </summary>
        public MessageService(ITransport transport, IMessageSerializer serializer, IMessageEncryptor encryptor, ILoggerProvider logger)
        {
            this.transport = transport;
            this.serializer = serializer;
            this.encryptor = encryptor;
            this.logger = logger;
        }

        void IMessageService.SetParticipantInfo(Guid connectionId, ParticipantInfo participant)
        {
            participants[connectionId] = participant;
        }

        async Task IMessageService.SendAsync(IMessage message, Guid roomId, Guid connectionId, CancellationToken cancellationToken = default)
        {
            var plainData = serializer.Serialize(message);
            byte[] dataToSend;

            if (message.RequiresEncryption)
            {
                if (!participants.TryGetValue(connectionId, out var recipient))
                {
                    throw new InvalidOperationException($"No participant info for connection {connectionId}");
                }
                var encrypted = encryptor.EncryptMessage(plainData, recipient.Id);
                dataToSend = encryptor.AddEncryptionHeader(encrypted);
            }
            else
            {
                dataToSend = encryptor.AddPlainHeader(plainData);
            }

            await transport.SendAsync(dataToSend, roomId, connectionId, cancellationToken);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            transport.RawMessageReceived += OnRawMessageReceived;
            await Task.CompletedTask;
        }

        private void OnRawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
        {
            try
            {
                byte[] plainData;
                var body = encryptor.RemoveEncryptionHeader(e.Data);

                if (encryptor.IsEncrypted(e.Data))
                {
                    if (!participants.TryGetValue(e.ConnectionId, out var senderParticipant))
                    {
                        // Если участник ещё не известен, возможно, это handshake – не шифруется
                        // Но для зашифрованных сообщений участник должен быть известен
                        logger.Log($"Received encrypted message from unknown connection {e.ConnectionId}, ignoring");
                        return;
                    }
                    plainData = encryptor.DecryptMessage(body, senderParticipant.Id);
                }
                else
                {
                    plainData = body;
                }

                var message = serializer.Deserialize(plainData);
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(e.RoomId, e.ConnectionId, message));
            }
            catch (Exception ex)
            {
                logger.Log($"Error processing raw message: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            cts?.Cancel();
            transport.RawMessageReceived -= OnRawMessageReceived;
            cts?.Dispose();
            await Task.CompletedTask;
        }
    }
}
