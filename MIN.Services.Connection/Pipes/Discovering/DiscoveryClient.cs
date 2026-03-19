using System.IO.Pipes;
using System.Net.NetworkInformation;
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

        async Task<DiscoveredRoom?> IDiscoveryClient.DiscoverRoomAsync(string targetPCName, TimeSpan timeout, CancellationToken cancellationToken)
        {
            await PingAsync(targetPCName, timeout, cancellationToken);

            var pipeName = DiscoveryPipeNameProvider.GetDiscoveryPipeName(targetPCName);

            using var pipe = new NamedPipeClientStream(
                targetPCName,
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough
            );

            var connectTask = pipe.ConnectAsync(timeout, cancellationToken);

            try
            {
                if (await Task.WhenAny(connectTask, Task.Delay(timeout, cancellationToken)) != connectTask)
                {
                    throw new RoomDiscoveryException($"Время подключение к {targetPCName} вышло");
                }

                logger.Log($"Опа, нашёл у {targetPCName} комнатку");

                if (await serializer.ReadMessageAsync(pipe, Guid.Empty, cancellationToken) is not DiscoveredRoom discoveryInfo)
                {
                    return null;
                }

                logger.Log($"Прочитал данные комнаты {targetPCName}");

                return discoveryInfo;
            }
            catch (Exception ex) when (ex is not RoomDiscoveryException)
            {
                throw new RoomDiscoveryException(ex.Message);
            }
        }

        private async Task PingAsync(string host, TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                using var pingSender = new Ping();
                var pingTask = pingSender.SendPingAsync(host, timeout, cancellationToken: cancellationToken);

                if (await Task.WhenAny(pingTask, Task.Delay(timeout, cancellationToken)) == pingTask)
                {
                    var reply = await pingTask;
                    if (reply.Status == IPStatus.Success)
                    {
                        return;
                    }
                    throw new RoomDiscoveryException(reply.Status.ToString());
                }
                throw new RoomDiscoveryException("Истекло время проверки хоста");
            }
            catch (Exception ex) when (ex is not RoomDiscoveryException)
            {
                throw new RoomDiscoveryException(ex.Message);
            }
        }

        public void Dispose() { }
    }
}
