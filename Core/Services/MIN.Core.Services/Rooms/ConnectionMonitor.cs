using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms
{
    /// <inheritdoc cref="IConnectionMonitor"/>
    public sealed class ConnectionMonitor : IConnectionMonitor, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IEventBus eventBus;
        private readonly IRoomRegistry roomRegistry;
        private readonly ILoggerProvider logger;
        private CancellationTokenSource? cts;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConnectionMonitor"/>
        /// </summary>
        public ConnectionMonitor(ITransport transport, IEventBus eventBus, IRoomRegistry roomRegistry, ILoggerProvider logger)
        {
            this.transport = transport;
            this.eventBus = eventBus;
            this.roomRegistry = roomRegistry;
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
                Guid roomId;

                try
                {
                    roomId = roomRegistry.GetRoomIdByConnectionId(e.ConnectionId);
                }
                catch
                {
                    return;
                }

                await eventBus.PublishAsync(new ConnectionStatusChangedEvent
                {
                    RoomId = roomId,
                    ConnectionId = e.ConnectionId,
                    ErrorMessage = e.Reason,
                    IsConnected = e.IsConnected
                }, cts?.Token ?? CancellationToken.None);
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
                cts = null;
            }
            transport.ConnectionStateChanged -= OnConnectionStateChanged;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync() => await StopAsync();
    }
}
