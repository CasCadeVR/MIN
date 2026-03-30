using System.Collections.Concurrent;
using MIN.Handlers.Contracts.Models;
using MIN.Handlers.Contracts.Dispatcher;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Transport.Contracts.Interfaces;
using MIN.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Events;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;

namespace MIN.Core.Services.Messaging
{
    /// <inheritdoc cref="IMessageService"/>
    public sealed class MessageService : IMessageService, IAsyncDisposable
    {
        private readonly IMessageDispatcher dispatcher;
        private readonly ILoggerProvider logger;

        private readonly IMessageReceiver receiver;
        private readonly IMessageSender sender;

        private readonly ConcurrentDictionary<Guid, ParticipantInfo> participantsByConnectionId = new();
        private CancellationTokenSource? cts;
        private bool disposed;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageService"/>
        /// </summary>
        public MessageService(ITransport transport,
            IMessageDispatcher dispatcher,
            IMessageSerializer serializer,
            IMessageEncryptor encryptor,
            ILoggerProvider logger)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;

            receiver = new MessageReceiver(transport, serializer, encryptor, logger, participantsByConnectionId);
            sender = new MessageSender(transport, serializer, encryptor, logger, participantsByConnectionId);
        }

        void IMessageService.SetParticipantInfo(Guid connectionId, ParticipantInfo participant)
        {
            participantsByConnectionId[connectionId] = participant;
        }

        async Task IMessageSender.SendAsync(IMessage message, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
        {
            await sender.SendAsync(message, roomId, connectionId, cancellationToken);
        }

        async Task IMessageSender.BroadcastAsync(IMessage message, Guid roomId, IEnumerable<Guid>? excludeConnections, CancellationToken cancellationToken)
        {
            await sender.BroadcastAsync(message, roomId, excludeConnections, cancellationToken);
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken = default)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            receiver.MessageReceived += OnMessageReceived;
            await receiver.StartListeningAsync(cts.Token);
        }

        private async void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            try
            {
                await dispatcher.DispatchAsync(e.Message, new MessageContext(e.Sender, e.RoomId, e.ConnectionId, cts!.Token));
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время обработки raw message: {ex.Message}");
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
            await (receiver as MessageReceiver)!.DisposeAsync();
            await (sender as MessageSender)!.DisposeAsync();
            cts?.Dispose();
            await Task.CompletedTask;
        }
    }
}
