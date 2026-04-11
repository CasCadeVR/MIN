using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Stores;
using MIN.Discovery.Events;
using MIN.Discovery.Messaging;
using MIN.Discovery.Services.Contracts.Exceptions;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Discovery.Transport.Contracts;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Discovery.Services
{
    /// <inheritdoc cref="IDiscoveryService"/>
    public sealed class DiscoveryService : IDiscoveryService
    {
        private readonly IDiscoveryTransport discoveryTransport;
        private readonly IMessageSerializer serializer;
        private readonly IRoomStore roomStore;
        private readonly IParticipantStore participantStore;
        private readonly IEventBus eventBus;
        private readonly ILoggerProvider logger;
        private CancellationTokenSource? serviceCts;
        private Guid roomId;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DiscoveryService"/>
        /// </summary>
        public DiscoveryService(
            IDiscoveryTransport discoveryTransport,
            IMessageSerializer serializer,
            IRoomStore roomStore,
            IParticipantStore participantStore,
            IEventBus eventBus,
            ILoggerProvider logger)
        {
            this.discoveryTransport = discoveryTransport;
            this.serializer = serializer;
            this.roomStore = roomStore;
            this.participantStore = participantStore;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        async Task IDiscoveryService.StartDiscoveryAsync(Guid roomId, CancellationToken cancellationToken)
        {
            if (serviceCts != null)
            {
                return;
            }

            this.roomId = roomId;
            serviceCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            discoveryTransport.MessageReceived += OnRequestReceived;
            await discoveryTransport.StartListeningAsync(serviceCts.Token);
        }

        /// <inheritdoc />
        public async Task StopDiscoveryAsync()
        {
            if (serviceCts == null)
            {
                return;
            }
            await serviceCts.CancelAsync();
            discoveryTransport.MessageReceived -= OnRequestReceived;
            await discoveryTransport.StopListeningAsync();
            serviceCts.Dispose();
            serviceCts = null;
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

            discoveryTransport.MessageReceived += OnResponseReceived;

            try
            {
                var tasks = computers
                   .Select(computer =>
                   {
                       try
                       {
                           return discoveryTransport.SendAsync(requestData, computer, timeout, cts.Token);
                       }
                       catch (Exception ex)
                       {
                           logger.Log($"Не удалось отправить запрос на обнаружение компу {computer}: {ex.Message}");
                       }

                       return Task.CompletedTask;
                   });

                await Task.WhenAll(tasks);
            }
            catch (DiscoveryException) { }
            catch (OperationCanceledException) { }
            finally
            {
                discoveryTransport.MessageReceived -= OnResponseReceived;
            }
        }

        private void OnResponseReceived(object? sender, DiscoveryRawMessageReceivedEventArgs e)
        {
            try
            {
                var message = serializer.Deserialize(e.Data);
                if (message is DiscoveryResponseMessage response)
                {
                    logger.Log($"Нашёл комнату: {response.Room.Name}");
                    eventBus.PublishAsync(new RoomDiscoveredEvent()
                    {
                        Room = response.Room,
                    });
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

                var roomInfo = new RoomInfo(room);
                roomInfo.ParticipantCount = participantStore.GetParticipants(roomId).Count();

                var discoveryResponse = new DiscoveryResponseMessage { Room = roomInfo };
                var data = serializer.Serialize(discoveryResponse);

                discoveryTransport.ResponseWithData(data, e.ConnectionId, serviceCts!.Token);
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка во время обработки запроса на обнаружение: {ex.Message}");
            }
        }

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
        public async ValueTask DisposeAsync() => await StopDiscoveryAsync();
    }
}
