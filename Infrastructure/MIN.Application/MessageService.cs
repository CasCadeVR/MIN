using System.Collections.Concurrent;
using MIN.Handlers.Contracts.Models;
using MIN.Application.Contracts.Interfaces;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Handlers.Contracts.Dispatcher;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Serialization.Contracts;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Events;
using MIN.Transport.Contracts.Interfaces;

namespace MIN.Application
{
    /// <inheritdoc cref="IMessageService"/>
    public sealed class MessageService : IMessageService, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IMessageDispatcher dispatcher;
        private readonly IMessageSerializer serializer;
        private readonly IMessageEncryptor encryptor;
        private readonly ILoggerProvider logger;
        private readonly ConcurrentDictionary<Guid, ParticipantInfo> participants = new();
        private CancellationTokenSource? cts;
        private bool disposed;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageService"/>
        /// </summary>
        public MessageService(ITransport transport,
            IMessageDispatcher dispatcher,
            IMessageSerializer serializer,
            IMessageEncryptor encryptor,
            ILoggerProvider logger)
        {
            this.transport = transport;
            this.dispatcher = dispatcher;
            this.serializer = serializer;
            this.encryptor = encryptor;
            this.logger = logger;
        }

        void IMessageService.SetParticipantInfo(Guid connectionId, ParticipantInfo participant)
        {
            participants[connectionId] = participant;
        }

        async Task IMessageService.SendAsync(IMessage message, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
        {
            var dataToSend = SerializeAndEncryptMessage(message, connectionId);
            await transport.SendAsync(dataToSend, roomId, connectionId, cancellationToken);
        }

        async Task IMessageService.BroadcastAsync(IMessage message, Guid roomId, IEnumerable<Guid>? excludeConnections, CancellationToken cancellationToken)
        {
            var tasks = participants.Keys
                .Where(c => excludeConnections == null || !excludeConnections.Contains(c))
                .Select(connectionId => transport.SendAsync(SerializeAndEncryptMessage(message, connectionId),
                    roomId, connectionId, cancellationToken));

            await Task.WhenAll(tasks);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            transport.RawMessageReceived += async (sender, e)
                => await OnRawMessageReceived(sender, e);
            await Task.CompletedTask;
        }

        private async Task OnRawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
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
                var participant = participants[e.ConnectionId];

                await dispatcher.DispatchAsync(message, new MessageContext(participantInfo!, e.RoomId, e.ConnectionId, cts!.Token));
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время обработки raw message: {ex.Message}");
            }
        }

        private byte[] SerializeAndEncryptMessage(IMessage message, Guid connectionId)
        {
            var plainData = serializer.Serialize(message);
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

        public async ValueTask DisposeAsync()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            cts?.Cancel();
            transport.RawMessageReceived -= async (sender, e)
                => await OnRawMessageReceived(sender, e);
            cts?.Dispose();
            await Task.CompletedTask;
        }
    }
}
