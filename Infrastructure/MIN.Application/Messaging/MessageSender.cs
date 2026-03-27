using System.Collections.Concurrent;
using MIN.Services.Contracts.Interfaces.Messaging;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Entities.Contracts.Entities;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Serialization.Contracts;
using MIN.Services.Helpers.Contracts.Interfaces;
using MIN.Transport.Contracts.Interfaces;

namespace MIN.Services.Messaging
{
    /// <inheritdoc cref="IMessageSender"/>
    internal sealed class MessageSender : IMessageSender, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IMessageEncryptor encryptor;
        private readonly IMessageSerializer serializer;
        private readonly ILoggerProvider logger;
        private readonly ConcurrentDictionary<Guid, ParticipantInfo> participants = new();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageSender"/>
        /// </summary>
        public MessageSender(ITransport transport,
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

        async Task IMessageSender.SendAsync(IMessage message, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
        {
            var serialized = serializer.Serialize(message);
            var dataToSend = EncryptData(message, serialized, connectionId);
            await transport.SendAsync(dataToSend, roomId, connectionId, cancellationToken);
        }

        async Task IMessageSender.BroadcastAsync(IMessage message, Guid roomId, IEnumerable<Guid>? excludeConnections, CancellationToken cancellationToken)
        {
            var serialized = serializer.Serialize(message);

            var tasks = participants.Keys
                .Where(c => excludeConnections == null || !excludeConnections.Contains(c))
                .Select(connectionId => transport.SendAsync(EncryptData(message, serialized, connectionId),
                    roomId, connectionId, cancellationToken));

            await Task.WhenAll(tasks);
        }

        private byte[] EncryptData(IMessage message, byte[] plainData, Guid connectionId)
        {
            byte[] resultBytes;

            if (message.RequiresEncryption)
            {
                if (!participants.TryGetValue(connectionId, out var recipient))
                {
                    throw new InvalidOperationException($"No participant info for connection {connectionId}");
                }
                var encrypted = encryptor.EncryptMessage(plainData, recipient.Id);
                resultBytes = encryptor.AddEncryptionHeader(encrypted);
            }
            else
            {
                resultBytes = encryptor.AddPlainHeader(plainData);
            }

            return resultBytes;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
