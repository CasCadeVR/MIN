using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms
{
    /// <inheritdoc cref="IRoomHoster"/>
    public sealed class RoomHoster : IRoomHoster
    {
        private readonly ITransport transport;
        private readonly HashSet<Guid> activeRooms = new();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomHoster"/>
        /// </summary>
        public RoomHoster(ITransport transport)
        {
            this.transport = transport;
        }

        async Task IRoomHoster.StartHostingAsync(Guid roomId, IEndpoint endpoint, CancellationToken cancellationToken)
        {
            if (activeRooms.Contains(roomId))
            {
                return;
            }

            await transport.StartHostingAsync(roomId, endpoint, cancellationToken);
            activeRooms.Add(roomId);
        }

        async Task IRoomHoster.StopHostingAsync(Guid roomId)
        {
            if (!activeRooms.Contains(roomId))
            {
                return;
            }

            await transport.StopHostingAsync(roomId);
            activeRooms.Remove(roomId);
        }

        bool IRoomHoster.IsHosting(Guid roomId)
            => activeRooms.Contains(roomId);
    }
}
