using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Rooms;

namespace MIN.Core.Services.Messaging
{
    /// <inheritdoc cref="IMessageSender"/>
    public sealed class MessageSender : IMessageSender, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IMessageEncryptor encryptor;
        private readonly IMessageSerializer serializer;
        private readonly ILoggerProvider logger;
        private readonly IRoomService roomService;
        private readonly IParticipantRegistry participantService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageSender"/>
        /// </summary>
        public MessageSender(ITransport transport,
            IMessageSerializer serializer,
            IMessageEncryptor encryptor,
            ILoggerProvider logger,
            IRoomService roomService,
            IParticipantRegistry participantService)
        {
            this.transport = transport;
            this.serializer = serializer;
            this.encryptor = encryptor;
            this.logger = logger;
            this.roomService = roomService;
            this.participantService = participantService;
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

            var participants = roomService.GetCurrentParticipants(roomId);

            var tasks = participants
                .Select(x => participantService.GetConnectionIdFromParticipantId(x.Id))
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
                if (!participantService.TryGetParticipantInfo(connectionId, out var recipient))
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
