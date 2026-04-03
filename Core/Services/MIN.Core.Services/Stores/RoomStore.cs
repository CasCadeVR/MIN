using System.Collections.Concurrent;
using MIN.Core.Entities;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Core.Services.Stores
{
    /// <inheritdoc cref="IRoomStore"/>
    public sealed class RoomStore : IRoomStore
    {
        private readonly ConcurrentDictionary<Guid, Room> roomsById = new();

        Room IRoomStore.GetRoom(Guid roomId)
        {
            if (roomsById.TryGetValue(roomId, out var room))
            {
                return room;
            }

            throw new InvalidOperationException($"Комнаты с {roomId} не нашлось");
        }

        bool IRoomStore.TryGetRoom(Guid roomId, out Room room)
            => roomsById.TryGetValue(roomId, out room);

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
