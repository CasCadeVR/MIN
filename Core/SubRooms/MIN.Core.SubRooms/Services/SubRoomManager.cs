using System.Collections.Concurrent;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Core.SubRooms.Contracts.Models;

namespace MIN.Core.SubRooms.Services;

/// <inheritdoc cref="ISubRoomManager"/>
public class SubRoomManager : ISubRoomManager
{
    private readonly ConcurrentDictionary<Guid, SubRoomState> rooms = new();

    /// <inheritdoc />
    public SubRoomInfo HostSubRoom(Guid roomId, Guid creatorId, SubRoomPurpose purpose)
    {
        var room = rooms.GetOrAdd(roomId, _ => new SubRoomState());

        SubRoomInfo subRoom;
        lock (room)
        {
            subRoom = new SubRoomInfo
            {
                Id = room.NextId++,
                Purpose = purpose,
                CreatorId = creatorId,
                Participants = [],
                CreatedAt = DateTime.Now
            };
            room.SubRooms[subRoom.Id] = subRoom;
        }

        return subRoom;
    }

    /// <inheritdoc />
    public bool TryJoinSubRoom(Guid roomId, int subRoomId, Guid participantId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return false;
        }

        lock (room)
        {
            if (!room.SubRooms.TryGetValue(subRoomId, out var subRoom))
            {
                return false;
            }

            if (subRoom.Participants.Any(p => p.Id == participantId))
            {
                return false;
            }

            subRoom.Participants.Add(new ParticipantInfo { Id = participantId });
            return true;
        }
    }

    /// <inheritdoc />
    public void LeaveSubRoom(Guid roomId, int subRoomId, Guid participantId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return;
        }

        lock (room)
        {
            if (!room.SubRooms.TryGetValue(subRoomId, out var subRoom))
            {
                return;
            }

            subRoom.Participants.RemoveAll(p => p.Id == participantId);

            if (subRoom.Participants.Count == 0)
            {
                room.SubRooms.Remove(subRoomId);
            }
        }
    }

    /// <inheritdoc />
    public bool TryStopSubRoom(Guid roomId, int subRoomId, Guid requesterId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return false;
        }

        lock (room)
        {
            if (!room.SubRooms.TryGetValue(subRoomId, out var subRoom))
            {
                return false;
            }

            if (subRoom.CreatorId != requesterId)
            {
                return false;
            }

            room.SubRooms.Remove(subRoomId);
            return true;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<ParticipantInfo> GetParticipants(Guid roomId, int subRoomId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return [];
        }

        lock (room)
        {
            if (!room.SubRooms.TryGetValue(subRoomId, out var subRoom))
            {
                return [];
            }

            return subRoom.Participants.ToList().AsReadOnly();
        }
    }

    /// <inheritdoc />
    public SubRoomInfo? GetSubRoom(Guid roomId, int subRoomId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return null;
        }

        lock (room)
        {
            if (!room.SubRooms.TryGetValue(subRoomId, out var subRoom))
            {
                return null;
            }

            return subRoom with { Participants = subRoom.Participants.ToList() };
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<SubRoomInfo> GetRoomSubRooms(Guid roomId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return [];
        }

        lock (room)
        {
            return room.SubRooms.Values
                .Select(sr => sr with { Participants = [.. sr.Participants] })
                .ToList()
                .AsReadOnly();
        }
    }
}
