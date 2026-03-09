using System.IO.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Discovering;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models.Exceptions;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Pipes.Discovering
{
    /// <inheritdoc cref="IDiscoveryClient"/>
    public class DiscoveryClient : IDiscoveryClient, IDisposable
    {
        private readonly IPipeMessageSerializer serializer;
        private readonly ILoggerProvider logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DiscoveryClient"/>
        /// </summary>
        public DiscoveryClient(IPipeMessageSerializer serializer, ILoggerProvider logger)
        {
            this.serializer = serializer;
            this.logger = logger;
        }

        async Task<DiscoveredRoom?> IDiscoveryClient.DiscoverRoomAsync(string targetPCName, TimeSpan timeout)
        {
            var cts = new CancellationTokenSource(timeout);
            var pipeName = DiscoveryPipeNameProvider.GetDiscoveryPipeName(targetPCName);

            using var pipe = new NamedPipeClientStream(
                targetPCName,
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough
            );

            try
            {
                await pipe.ConnectAsync(cts.Token);

                logger.Log($"Опа, нашёл у {targetPCName} комнатку");

                if (await serializer.ReadMessageAsync(pipe, Guid.Empty, cts.Token) is not DiscoveredRoom discoveryInfo)
                {
                    return null;
                }

                return discoveryInfo;
            }
            catch (TimeoutException)
            {
                throw new RoomDiscoveryException($"Время подключение к {targetPCName} вышло");
            }
            catch (Exception ex) when (ex is not RoomDiscoveryException)
            {
                throw new RoomDiscoveryException(ex.Message);
            }
        }

        public void Dispose() { }
    }
}
