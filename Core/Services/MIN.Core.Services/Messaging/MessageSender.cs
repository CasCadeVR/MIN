using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Constants;
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
            IMessageEncryptor encryptor,
            IMessageSerializer serializer,
            IParticipantStore participantStore,
            IParticipantConnectionRegistry participantConnectionRegistry)
        {
            this.transport = transport;
            this.encryptor = encryptor;
            this.serializer = serializer;
            this.participantStore = participantStore;
            this.participantConnectionRegistry = participantConnectionRegistry;
        }

        async Task IMessageSender.SendAsync(IMessage message, Guid roomId, Guid senderId, Guid recipientConnectionId, CancellationToken cancellationToken)
        {
            var serialized = serializer.Serialize(message);
            var dataToSend = EncryptDataIfRequired(message, serialized, recipientConnectionId);

            await transport.SendAsync(dataToSend, roomId, recipientConnectionId, cancellationToken);
        }

        async Task IMessageSender.BroadcastAsync(IMessage message, Guid roomId, IEnumerable<Guid>? excludeConnectionIds, CancellationToken cancellationToken)
        {
            var serialized = serializer.Serialize(message);
            var participants = participantStore.GetParticipants(roomId);

            excludeConnectionIds = (excludeConnectionIds ?? [])
                .Append(CoreServicesConstants.LocalConnectionId);

            var tasks = participants
                .Select(participant => participantConnectionRegistry.GetConnectionIdFromParticipantId(participant.Id))
                .Where(connectionId => !excludeConnectionIds.Contains(connectionId))
                .Select(connectionId => transport.SendAsync(EncryptDataIfRequired(message, serialized, connectionId),
                    roomId, connectionId, cancellationToken));

            await Task.WhenAll(tasks);
        }

        private byte[] EncryptDataIfRequired(IMessage message, byte[] plainData, Guid recipientConnectionId)
        {
            byte[] resultBytes;

            if (message.RequiresEncryption)
            {
                var recipientId = participantConnectionRegistry.GetParticipantIdFromConnectionId(recipientConnectionId);
                var encrypted = encryptor.EncryptMessage(plainData, recipientId);
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
