using MIN.Discovery.Transport.Contracts;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Discovery.Transport.NamedPipes.Client;
using MIN.Discovery.Transport.NamedPipes.Server;
using MIN.Discovery.Transport.NamedPipes.Services;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Discovery.Transport.NamedPipes
{
    /// <inheritdoc cref="IDiscoveryTransport"/>
    public class NamedPipeDiscoveryTransport : IDiscoveryTransport
    {
        private readonly NamedPipeDiscoveryClient discoveryClient;
        private readonly NamedPipeDiscoveryServer discoveryServer;

        /// <inheritdoc />
        public event EventHandler<DiscoveryRawMessageReceivedEventArgs>? MessageReceived;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NamedPipeDiscoveryTransport"/>
        /// </summary>
        public NamedPipeDiscoveryTransport(ILocalNetworkComputerProvider localNetworkComputerProvider, ILoggerProvider logger)
        {
            var machineName = localNetworkComputerProvider.GetLocalMachineName();
            var pipeName = DiscoveryPipeNameProvider.GetDiscoveryPipeName(machineName);

            discoveryServer = new NamedPipeDiscoveryServer(pipeName, logger);
            discoveryServer.MessageReceived += (sender, e)
                => MessageReceived?.Invoke(sender, e);

            discoveryClient = new NamedPipeDiscoveryClient(pipeName, logger);
            discoveryClient.MessageReceived += (sender, e)
                => MessageReceived?.Invoke(sender, e);
        }

        async Task IDiscoveryTransport.StartListeningAsync(CancellationToken cancellationToken)
        {
            await discoveryServer.StartListeningAsync(cancellationToken);
        }

        async Task IDiscoveryTransport.ResponseWithData(byte[] responseData, Guid? connectionId, CancellationToken cancellationToken)
        {
            await discoveryServer.ResponseWithData(responseData, connectionId, cancellationToken);
        }

        async Task IDiscoveryTransport.StopListeningAsync()
        {
            await discoveryServer.StopListeningAsync();
        }

        public async Task SendAsync(byte[] data, string? destination, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            await discoveryClient.SendAsync(data, destination, timeout, cancellationToken);
        }
    }
}
