using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Constants;

namespace MIN.Core.Services.Rooms
{
    /// <inheritdoc cref="IConnectionMonitor"/>
    public sealed class ConnectionMonitor : IConnectionMonitor, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IEventBus eventBus;
        private readonly IMessageSender messageSender;
        private readonly IIdentityService identityService;
        private readonly ILoggerProvider logger;
        private CancellationTokenSource cts = null!;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConnectionMonitor"/>
        /// </summary>
        public ConnectionMonitor(ITransport transport,
            IEventBus eventBus,
            IMessageSender messageSender,
            IIdentityService identityService,
            ILoggerProvider logger)
        {
            this.transport = transport;
            this.eventBus = eventBus;
            this.messageSender = messageSender;
            this.identityService = identityService;
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
                if (!e.IsConnected)
                {
                    if (e.ConnectionId == CoreServicesConstants.LocalConnectionId)
                    {
                        return;
                    }

                    var participantLeftMessage = new ParticipantLeftMessage()
                    {
                        Participant = new ParticipantInfo(identityService.SelfPartcipant),
                        RoomId = e.RoomId,
                    };

                    await messageSender.BroadcastAsync(participantLeftMessage, e.RoomId, null, cts.Token);
                }

                await eventBus.PublishAsync(new ConnectionStatusChangedEvent
                {
                    RoomId = e.RoomId,
                    ConnectionId = e.ConnectionId,
                    ErrorMessage = e.Reason,
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
