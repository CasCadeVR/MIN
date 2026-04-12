using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Core.Services.Rooms
{
    /// <inheritdoc cref="IConnectionMonitor"/>
    public sealed class ConnectionMonitor : IConnectionMonitor, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IEventBus eventBus;
        private readonly IMessageRouter messageRouter;
        private readonly IRoomStore roomStore;
        private readonly IParticipantConnectionRegistry participantConnectionRegistry;
        private readonly ILoggerProvider logger;

        private CancellationTokenSource cts = null!;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConnectionMonitor"/>
        /// </summary>
        public ConnectionMonitor(ITransport transport,
            IEventBus eventBus,
            IMessageRouter messageRouter,
            IRoomStore roomStore,
            IParticipantConnectionRegistry participantConnectionRegistry,
            ILoggerProvider logger)
        {
            this.transport = transport;
            this.eventBus = eventBus;
            this.messageRouter = messageRouter;
            this.roomStore = roomStore;
            this.participantConnectionRegistry = participantConnectionRegistry;
            this.logger = logger;
        }

        async Task IConnectionMonitor.StartAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            transport.ConnectionStateChanged += OnConnectionStateChanged;
            await Task.CompletedTask;
        }

        private async void OnConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
        {
            try
            {
                var leavingMessage = e.LeavingMessage;
                var needToDisconnect = false;

                if (!e.IsConnected)
                {
                    participantConnectionRegistry.TryGetParticipantFromConnectionId(e.ConnectionId, out var leavingParticipant);

                    var hostParticipantId = roomStore.GetRoomHostParticipantId(e.RoomId);
                    var isHostLeaving = hostParticipantId == leavingParticipant.Id;

                    needToDisconnect = isHostLeaving;

                    if (isHostLeaving)
                    {
                        leavingMessage = !string.IsNullOrEmpty(e.LeavingMessage) ? leavingMessage : "Хост остановил комнату";
                    }
                    else
                    {
                        var participantLeftMessage = new ParticipantLeftMessage()
                        {
                            Participant = leavingParticipant,
                            RoomId = e.RoomId,
                        };

                        await messageRouter.RouteAsync(participantLeftMessage, e.RoomId, hostParticipantId, Recipient.FromEmpty(), cts.Token);
                    }
                }

                await eventBus.PublishAsync(new ConnectionStatusChangedEvent
                {
                    RoomId = e.RoomId,
                    ConnectionId = e.ConnectionId,
                    LeavingMessage = leavingMessage,
                    NeedToDisconnect = needToDisconnect,
                    IsConnected = e.IsConnected
                }, cts.Token);
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время обработки изменения состояния подключения: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            if (cts != null)
            {
                await cts.CancelAsync();
                cts.Dispose();
                cts = null!;
            }
            transport.ConnectionStateChanged -= OnConnectionStateChanged;
        }

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
        public async ValueTask DisposeAsync() => await StopAsync();
    }
}
