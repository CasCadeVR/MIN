using System.Collections.Concurrent;
using MIN.Core.Entities;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Core.Services.Stores
{
    /// <inheritdoc cref="IRoomStore"/>
    public sealed class RoomStore : IRoomStore
    {
        private readonly IParticipantStore participantStore;
        private readonly IMessageStore messageStore;
        private readonly ConcurrentDictionary<Guid, Room> roomsById = new();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomStore"/>
        /// </summary>
        public RoomStore(IParticipantStore participantStore, IMessageStore messageStore)
        {
            this.participantStore = participantStore;
            this.messageStore = messageStore;
        }

        bool IRoomStore.RoomExists(Guid roomId)
            => roomsById.ContainsKey(roomId);

        Room IRoomStore.GetRoom(Guid roomId)
        {
            if (roomsById.TryGetValue(roomId, out var room))
            {
                room.CurrentParticipants = participantStore.GetParticipants(roomId).ToList();
                room.ChatHistory = messageStore.GetHistory(roomId).ToList();
                return room;
            }

            throw new InvalidOperationException($"Комнаты с {roomId} не нашлось");
        }

        bool IRoomStore.TryGetRoom(Guid roomId, out Room room)
        {
            if (roomsById.TryGetValue(roomId, out room!))
            {
                room.CurrentParticipants = participantStore.GetParticipants(roomId).ToList();
                room.ChatHistory = messageStore.GetHistory(roomId).ToList();
                return true;
            }

            return false;
        }

        Guid IRoomStore.GetRoomHostParticipantId(Guid roomId)
            => roomsById.TryGetValue(roomId, out var room) ? room.HostParticipant.Id : throw new KeyNotFoundException();

        void IRoomStore.Add(Room room)
        {
            roomsById[room.Id] = room;
        }

        void IRoomStore.Remove(Guid roomId)
        {
            roomsById.TryRemove(roomId, out _);
        }

        IEnumerable<Room> IRoomStore.GetAllRooms()
            => roomsById.Values;
    }
}
