using MIN.Core.Events.Contracts;
using MIN.Core.Serialization.Contracts;
using MIN.Discovery.Events;
using MIN.Discovery.Messaging;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Discovery.Transport.Contracts;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Discovery.Services
{
    /// <inheritdoc cref="IDiscoveryService"/>
    public sealed class DiscoveryService : IDiscoveryService
    {
        private readonly IDiscoveryTransport discoveryTransport;
        private readonly IMessageSerializer serializer;
        private readonly IRoomStore roomStore;
        private readonly IEventBus eventBus;
        private readonly ILoggerProvider logger;
        private CancellationTokenSource? cts;
        private Guid roomId;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DiscoveryService"/>
        /// </summary>
        public DiscoveryService(
            IDiscoveryTransport discoveryTransport,
            IMessageSerializer serializer,
            IRoomStore roomStore,
            IEventBus eventBus,
            ILoggerProvider logger)
        {
            this.discoveryTransport = discoveryTransport;
            this.serializer = serializer;
            this.roomStore = roomStore;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        async Task IDiscoveryService.StartDiscoveryAsync(Guid roomId, CancellationToken cancellationToken)
        {
            if (cts != null)
            {
                return;
            }

            this.roomId = roomId;
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            discoveryTransport.MessageReceived += OnRequestReceived;
            await discoveryTransport.StartListeningAsync(cts.Token);
        }

        public async Task StopDiscoveryAsync()
        {
            if (cts == null)
            {
                return;
            }
            cts.Cancel();
            discoveryTransport.MessageReceived -= OnRequestReceived;
            await discoveryTransport.StopListeningAsync();
            cts.Dispose();
            cts = null;
        }

        async Task IDiscoveryService.DiscoverRoomsAsync(IEnumerable<string>? computers, TimeSpan timeout, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(computers);

            var request = new DiscoveryRequestMessage();
            var requestData = serializer.Serialize(request);

            if (!computers.Any())
            {
                logger.Log("No remote computers to discover");
                return;
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);

            discoveryTransport.MessageReceived += (sender, e)
                => OnResponseReceived(sender, e, cts.Token);

            try
            {
                foreach (var computer in computers)
                {
                    try
                    {
                        await discoveryTransport.SendAsync(requestData, computer, timeout, cts.Token);
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"Не удалось отправить запрос на обнаружение компу {computer}: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                discoveryTransport.MessageReceived -= (sender, e)
                    => OnResponseReceived(sender, e, cts.Token);
            }
        }

        private void OnResponseReceived(object? sender, DiscoveryRawMessageReceivedEventArgs e, CancellationToken cancellationToken)
        {
            try
            {
                var message = serializer.Deserialize(e.Data);
                if (message is DiscoveryResponseMessage response)
                {
                    eventBus.PublishAsync(new RoomDiscoveredEvent(response.Room), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Error parsing discovery response: {ex.Message}");
            }
        }

        private void OnRequestReceived(object? sender, DiscoveryRawMessageReceivedEventArgs e)
        {
            try
            {
                var message = serializer.Deserialize(e.Data);

                if (message is not DiscoveryRequestMessage _)
                {
                    return;
                }

                var room = roomStore.GetRoom(roomId);

                if (room == null)
                {
                    logger.Log($"Получил запрос на обнаружение, но комната не была установлена", LogLevel.Warning);
                    return;
                }

                var hostingRoomInfo = new RoomInfo()
                {
                    Id = room.Id,
                    Name = room.Name,
                    HostParticipant = room.HostParticipant,
                    ParticipantCount = room.ParticipantCount,
                    IsActive = room.IsActive,
                    MaximumParticipants = room.MaximumParticipants,
                };

                var discoveryResponse = new DiscoveryResponseMessage { Room = hostingRoomInfo };
                var data = serializer.Serialize(discoveryResponse);

                discoveryTransport.ResponseWithData(data, timeout: null, cts!.Token);
            }
            catch (Exception ex)
            {
                logger.Log($"Error processing discovery message: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync() => await StopDiscoveryAsync();
    }
}
