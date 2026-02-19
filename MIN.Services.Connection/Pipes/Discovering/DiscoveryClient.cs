using System.IO.Pipes;
using System.Security.Principal;
using MIN.Services.Connection.Contracts.Interfaces.Discovering;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models.Exceptions;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Pipes.Discovering
{
    /// <inheritdoc cref="IDiscoveryClient"/>
    public class DiscoveryClient : IDiscoveryClient, IAsyncDisposable
    {
        private readonly IPipeMessageSerializer serializer;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DiscoveryClient"/>
        /// </summary>
        /// <param name="serializer"></param>
        public DiscoveryClient(IPipeMessageSerializer serializer)
        {
            this.serializer = serializer;
        }

        async Task<Room?> IDiscoveryClient.DiscoverRoomAsync(string targetPCName, TimeSpan timeout)
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

                if (await serializer.ReadMessageAsync(pipe, cts.Token) is not DiscoveredRoom discoveryInfo)
                {
                    return null;
                }

                foreach (var participant in discoveryInfo.CurrentParticipants)
                {
                    discoveryInfo.Room.AddParticipant(participant);
                }

                return discoveryInfo.Room;
            }
            catch (TimeoutException)
            {
                throw new RoomDiscoveryException($"Timeout connecting to {targetPCName}");
            }
            catch (Exception ex) when (ex is not RoomDiscoveryException)
            {
                throw new RoomDiscoveryException($"Failed to discover room on {targetPCName}: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync() { }
    }
}
