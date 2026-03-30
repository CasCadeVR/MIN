using System.Collections.Concurrent;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Rooms;

namespace MIN.Core.Services.Rooms
{
    /// <inheritdoc cref="IRoomRegistry"/>
    public sealed class RoomRegistry : IRoomRegistry
    {
        private readonly ConcurrentDictionary<Guid, Room> roomsById = new();
        private readonly ConcurrentDictionary<Guid, Guid> roomIdByConnectionId = new();

        public Room GetRoom(Guid roomId)
        {
            if (roomsById.TryGetValue(roomId, out var room))
            {
                return room;
            }

            throw new InvalidOperationException($"Комнаты с {roomId} не нашлось");
        }

        bool IRoomRegistry.TryGetRoom(Guid roomId, out Room room)
        {
            return roomsById.TryGetValue(roomId, out room);
        }

        void IRoomRegistry.RegisterRoom(Guid connectionId, Room room)
        {
            roomsById[room.Id] = room;
            roomIdByConnectionId[connectionId] = room.Id;
        }

        void IRoomRegistry.UnregisterRoom(Guid connectionId)
        {
            if (roomIdByConnectionId.TryRemove(connectionId, out var roomId))
            {
                roomsById.TryRemove(roomId, out _);
            }
        }

        void IRoomRegistry.AddParticipant(Guid roomId, ParticipantInfo participant)
        {
            if (!roomsById.TryGetValue(roomId, out var room))
            {
                throw new InvalidOperationException($"Комнаты с {roomId} не нашлось");
            }

            room.CurrentParticipants.Add(participant);
        }

        void IRoomRegistry.RemoveParticipant(Guid roomId, Guid participantId)
        {
            if (!roomsById.TryGetValue(roomId, out var room))
            {
                throw new InvalidOperationException($"Комнаты с {roomId} не нашлось");
            }

            var participant = room.CurrentParticipants.FirstOrDefault(p => p.Id == participantId);
            if (participant != null)
            {
                room.CurrentParticipants.Remove(participant);
            }
        }

        Guid IRoomRegistry.GetRoomIdByConnectionId(Guid connectionId)
        {
            if (roomIdByConnectionId.TryGetValue(connectionId, out var roomId))
            {
                return roomId;
            }

            throw new InvalidOperationException($"Room not found for connection {connectionId}");
        }

        IEnumerable<Room> IRoomRegistry.GetAllRooms()
            => roomsById.Values;

        IEnumerable<ParticipantInfo> IRoomRegistry.GetCurrentParticipants(Guid roomId)
        {
            if (!roomsById.TryGetValue(roomId, out var room))
            {
                throw new InvalidOperationException($"Комнаты с {roomId} не нашлось");
            }

            return room.CurrentParticipants.Select(participantData => new ParticipantInfo(participantData));
        }
    }
}
