using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Stores;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Services.Messaging
{
    /// <inheritdoc cref="IMessageSender"/>
    public sealed class MessageSender : IMessageSender, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IMessageEncryptor encryptor;
        private readonly IMessageSerializer serializer;
        private readonly IParticipantStore participantStore;
        private readonly IParticipantConnectionRegistry participantConnectionRegistry;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageSender"/>
        /// </summary>
        public MessageSender(ITransport transport,
            IMessageSerializer serializer,
            IMessageEncryptor encryptor,
            IParticipantStore participantStore,
            IParticipantConnectionRegistry participantConnectionRegistry)
        {
            this.transport = transport;
            this.serializer = serializer;
            this.encryptor = encryptor;
            this.participantStore = participantStore;
            this.participantConnectionRegistry = participantConnectionRegistry;
        }

        async Task IMessageSender.SendAsync(IMessage message, Guid roomId, Guid? senderId, Guid? recipientConnectionId, CancellationToken cancellationToken)
        {
            if (message.IsPublic)
            {
                await BroadcastAsync(message, roomId, null, cancellationToken);
            }
            else
            {
                if (!recipientConnectionId.HasValue)
                {
                    throw new InvalidOperationException("RecipientConnectionId required for private message");
                }

                var serialized = serializer.Serialize(message);
                var dataToSend = EncryptData(message, serialized, (Guid)recipientConnectionId);

                await transport.SendAsync(dataToSend, roomId, (Guid)recipientConnectionId, cancellationToken);
            }
        }

        public async Task BroadcastAsync(IMessage message, Guid roomId, IEnumerable<Guid>? excludeConnections, CancellationToken cancellationToken)
        {
            var serialized = serializer.Serialize(message);

            var participants = participantStore.GetParticipants(roomId);

            var tasks = participants
                .Select(x => participantConnectionRegistry.GetConnectionIdFromParticipantId(x.Id))
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
                if (!participantConnectionRegistry.TryGetParticipantFromConnectionId(connectionId, out var recipient))
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

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
