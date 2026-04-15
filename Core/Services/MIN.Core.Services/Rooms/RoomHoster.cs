using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms
{
    /// <inheritdoc cref="IRoomHoster"/>
    public sealed class RoomHoster : IRoomHoster
    {
        private readonly IMessageStore messageStore;
        private readonly ITransport transport;
        private readonly HashSet<Guid> activeRooms = new();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomHoster"/>
        /// </summary>
        public RoomHoster(IMessageStore messageStore, ITransport transport)
        {
            this.messageStore = messageStore;
            this.transport = transport;
        }

        async Task IRoomHoster.StartHostingAsync(RoomInfo roomInfo, CancellationToken cancellationToken)
        {
            if (activeRooms.Contains(roomInfo.Id))
            {
                return;
            }

            messageStore.AddMessage(roomInfo.Id, new SystemTextMessage()
            {
                Content = $"Комната {roomInfo.Name} была создана в {DateTime.Now.ToShortTimeString()}",
            });

            messageStore.AddMessage(roomInfo.Id, new ParticipantJoinedMessage()
            {
                Participant = roomInfo.HostParticipant,
                RoomId = roomInfo.Id
            });

            await transport.StartHostingAsync(roomInfo.Id, cancellationToken);
            activeRooms.Add(roomInfo.Id);
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
