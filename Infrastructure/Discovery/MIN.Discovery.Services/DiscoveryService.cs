using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Serialization.Contracts;
using MIN.Discovery.Events;
using MIN.Discovery.Messaging;
using MIN.Discovery.Services.Contracts.Interfaces;
using MIN.Discovery.Transport.Contracts;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Discovery.Services
{
    /// <inheritdoc cref="IDiscoveryService"/>
    public sealed class NamedPipeDiscoveryService : IDiscoveryService
    {
        private readonly IDiscoveryTransport discoveryTransport;
        private readonly IMessageSerializer serializer;
        private readonly IEventBus eventBus;
        private readonly ILocalNetworkComputerProvider computerProvider;
        private readonly ILoggerProvider logger;
        private readonly string localMachineName;
        private CancellationTokenSource? cts;
        private RoomInfo? hostingRoomInfo;
        private string? searchZone;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NamedPipeDiscoveryService"/>
        /// </summary>
        public NamedPipeDiscoveryService(
            IDiscoveryTransport discoveryTransport,
            IMessageSerializer serializer,
            IEventBus eventBus,
            ILocalNetworkComputerProvider computerProvider,
            ILoggerProvider logger)
        {
            this.discoveryTransport = discoveryTransport;
            this.serializer = serializer;
            this.eventBus = eventBus;
            this.computerProvider = computerProvider;
            this.logger = logger;
            localMachineName = Environment.MachineName;
        }

        async Task IDiscoveryService.StartDiscoveryAsync(RoomInfo roomInfo, CancellationToken cancellationToken)
        {
            if (cts != null)
            {
                return;
            }

            hostingRoomInfo = roomInfo;
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            discoveryTransport.MessageReceived += OnMessageReceived;
            await discoveryTransport.StartListeningAsync(cts.Token);
        }

        public async Task StopDiscoveryAsync()
        {
            if (cts == null)
            {
                return;
            }
            cts.Cancel();
            discoveryTransport.MessageReceived -= OnMessageReceived;
            await discoveryTransport.StopListeningAsync();
            cts.Dispose();
            cts = null;
        }

        async Task IDiscoveryService.DiscoverRoomsAsync(string? searchZone, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var request = new DiscoveryRequestMessage();
            var requestData = serializer.Serialize(request);

            discoveryTransport.MessageReceived += OnMessageReceived;

            try
            {
                var computers = computerProvider.GetLocalNetworkMachineNames(searchZone
                    ?? throw new ArgumentNullException("Для discovery нужен список имён компьютеров"));

                foreach (var computer in computers)
                {
                    if (string.Equals(computer, localMachineName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    try
                    {
                        await discoveryTransport.SendAsync(requestData, computer, timeout, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"Не удалось отправить запрос на обнаружение компу {computer}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка в пробеге по комьютерам в обнаружении: {ex.Message}");
            }
        }

        private void OnMessageReceived(object? sender, DiscoveryRawMessageReceivedEventArgs e)
        {
            try
            {
                var message = serializer.Deserialize(e.Data);
                if (message is DiscoveryResponseMessage response)
                {
                    eventBus.PublishAsync(new RoomDiscoveredEvent(response.Room), cts!.Token);
                    discoveryTransport.MessageReceived -= OnMessageReceived;
                }
                else if (message is DiscoveryRequestMessage _)
                {
                    if (hostingRoomInfo == null)
                    {
                        logger.Log($"Получил запрос на обнаружение, но комната не была установлена", LogLevel.Warning);
                        return;
                    }

                    var discoveryResponse = new DiscoveryResponseMessage { Room = hostingRoomInfo };
                    var data = serializer.Serialize(discoveryResponse);

                    discoveryTransport.ResponseWithData(data, timeout: null, cts!.Token);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Error processing discovery message: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync() => await StopDiscoveryAsync();
    }
}
