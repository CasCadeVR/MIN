using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Discovery.Events;
using MIN.Discovery.Messaging;
using MIN.Discovery.Services.Contracts.Exceptions;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Discovery.Services.Contracts.Models;
using MIN.Discovery.Transport.Contracts;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Discovery.Services
{
    /// <inheritdoc cref="IDiscoveryService"/>
    public sealed class DiscoveryService : IDiscoveryService
    {
        private readonly ITransport transport;
        private readonly IDiscoveryTransport discoveryTransport;
        private readonly IMessageSerializer serializer;
        private readonly IRoomStore roomStore;
        private readonly IRoomFactory roomFactory;
        private readonly IEventBus eventBus;
        private readonly ILoggerProvider logger;
        private readonly HashSet<Guid> activeRoomIds = [];
        private CancellationTokenSource? serviceCts;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DiscoveryService"/>
        /// </summary>
        public DiscoveryService(
            ITransport transport,
            IDiscoveryTransport discoveryTransport,
            IMessageSerializer serializer,
            IRoomStore roomStore,
            IRoomFactory roomFactory,
            IEventBus eventBus,
            ILoggerProvider logger)
        {
            this.transport = transport;
            this.discoveryTransport = discoveryTransport;
            this.serializer = serializer;
            this.roomStore = roomStore;
            this.roomFactory = roomFactory;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        async Task IDiscoveryService.StartDiscoveryAsync(Guid roomId, CancellationToken cancellationToken)
        {
            activeRoomIds.Add(roomId);

            if (serviceCts != null)
            {
                return;
            }

            serviceCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            discoveryTransport.MessageReceived += OnRequestReceived;
            await discoveryTransport.StartListeningAsync(serviceCts.Token);
        }

        /// <inheritdoc />
        public async Task StopDiscoveryAsync(Guid roomId)
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
            activeRoomIds.Remove(roomId);
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
            discoveryTransport.MessageReceived += OnResponseReceived;

            logger.Log("[DEBUG]: starting discovery");

            try
            {
                var tasks = computers
                .Select(async computer =>
                {
                    try
                    {
                        using var perComputerCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
                        perComputerCts.CancelAfter(timeout);
                        await discoveryTransport.SendAsync(requestData, computer, timeout, perComputerCts.Token);
                        return eventBus.PublishAsync(new EndpointCheckedEvent()
                        {
                            Endpoint = computer
                        }, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"Не удалось отправить запрос на обнаружение компу {computer}: {ex.Message}");
                        return Task.CompletedTask;
                    }
                });

                await Task.WhenAll(tasks);
            }
            catch (DiscoveryException ex)
            {
                logger.Log($"[DEBUG]: discovery failed at discoveryException: {ex.Message}");
            }
            catch (OperationCanceledException ex)
            {
                logger.Log($"[DEBUG]: discovery failed at OperationCanceledException: {ex.Message}");
            }
            finally
            {
                discoveryTransport.MessageReceived -= OnResponseReceived;
                logger.Log("[DEBUG]: discovery ended");
            }
        }

        private void OnResponseReceived(object? sender, DiscoveryRawMessageReceivedEventArgs e)
        {
            try
            {
                var message = serializer.Deserialize(e.Data);
                if (message is DiscoveryResponseMessage response)
                {
                    logger.Log($"Нашёл +{response.RoomDiscoveryInfos.Count} комнат");
                    eventBus.PublishAsync(new RoomDiscoveredEvent()
                    {
                        MachineName = e.MachineName,
                        RoomDiscoveryInfos = response.RoomDiscoveryInfos,
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

                var discoveryResponse = new DiscoveryResponseMessage();

                foreach (var roomId in activeRoomIds)
                {
                    var room = roomStore.GetRoom(roomId);

                    if (room == null)
                    {
                        logger.Log($"Получил запрос на обнаружение, но комната не была установлена", LogLevel.Warning);
                        return;
                    }

                    var roomInfo = new RoomInfo(room)
                    {
                        ParticipantCount = roomFactory.GetOrCreateContext(roomId).Participants.GetParticipants().Count()
                    };

                    discoveryResponse.RoomDiscoveryInfos.Add(new RoomDiscoveryInfo()
                    {
                        Room = roomInfo,
                        Endpoint = transport.GetEndpoint(roomId),
                    });
                }

                var data = serializer.Serialize(discoveryResponse);

                discoveryTransport.ResponseWithData(data, e.ConnectionId, serviceCts!.Token);
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка во время обработки запроса на обнаружение: {ex.Message}");
            }
        }
    }
}
