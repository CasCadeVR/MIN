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
            discoveryClient = new NamedPipeDiscoveryClient(pipeName, logger);
        }

        async Task IDiscoveryTransport.StartListeningAsync(CancellationToken cancellationToken)
        {
            discoveryServer.MessageReceived += MessageReceived;
            await discoveryServer.StartListeningAsync(cancellationToken);
        }

        async Task IDiscoveryTransport.ResponseWithData(byte[] responseData, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            await discoveryServer.ResponseWithData(responseData, timeout, cancellationToken);
        }

        async Task IDiscoveryTransport.StopListeningAsync()
        {
            await discoveryServer.StopListeningAsync();
        }

        public async Task SendAsync(byte[] data, string? destination, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            discoveryClient.MessageReceived += MessageReceived;
            await discoveryClient.SendAsync(data, destination, timeout, cancellationToken);
        }
    }
}
