using System.Collections.Concurrent;
using MIN.Core.Entities;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.Stores.Services;

/// <inheritdoc cref="IRoomStore"/>
public sealed class RoomStore : IRoomStore
{
    private readonly IRoomFactory roomFactory;
    private readonly ConcurrentDictionary<Guid, Room> roomsById = new();

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomStore"/>
    /// </summary>
    public RoomStore(IRoomFactory roomFactory)
    {
        this.roomFactory = roomFactory;
    }

    bool IRoomStore.RoomExists(Guid roomId)
        => roomsById.ContainsKey(roomId);

    Room IRoomStore.GetRoom(Guid roomId)
    {
        if (roomsById.TryGetValue(roomId, out var room))
        {
            var context = roomFactory.GetOrCreateContext(roomId);
            room.CurrentParticipants = context.Participants.GetParticipants().ToList();
            room.ChatHistory = context.Messages.GetHistory().ToList();
            return room;
        }

        throw new InvalidOperationException($"Комнаты с {roomId} не нашлось");
    }

    bool IRoomStore.TryGetRoom(Guid roomId, out Room room)
    {
        if (roomsById.TryGetValue(roomId, out room!))
        {
            var context = roomFactory.GetOrCreateContext(roomId);
            room.CurrentParticipants = context.Participants.GetParticipants().ToList();
            room.ChatHistory = roomFactory.GetOrCreateContext(roomId).Messages.GetHistory().ToList();
            return true;
        }

        return false;
    }

    Room IRoomStore.GetRoomFor(Guid participantId, Guid roomId)
    {
        if (roomsById.TryGetValue(roomId, out var room))
        {
            var context = roomFactory.GetOrCreateContext(roomId);
            room.CurrentParticipants = context.Participants.GetParticipants().ToList();
            room.ChatHistory = context.Messages.GetHistory()
                .Where(x => x.IsPublic || x.RecipientId == participantId || x.SenderId == participantId)
                .ToList();
            return room;
        }

        throw new InvalidOperationException($"Комнаты с {roomId} не нашлось");
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
