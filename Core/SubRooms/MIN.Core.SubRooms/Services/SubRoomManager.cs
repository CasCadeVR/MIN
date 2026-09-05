using System.Collections.Concurrent;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Core.SubRooms.Contracts.Models;
using MIN.Core.SubRooms.Models;

namespace MIN.Core.SubRooms.Services;

/// <inheritdoc cref="ISubRoomManager"/>
public class SubRoomManager : ISubRoomManager
{
    private readonly ConcurrentDictionary<Guid, SubRoomState> rooms = new();

    SubRoomInfo ISubRoomManager.HostSubRoom(Guid roomId, ParticipantInfo creator, SubRoomPurpose purpose, int? maximum)
    {
        var room = rooms.GetOrAdd(roomId, _ => new SubRoomState());

        SubRoomInfo subRoom;
        lock (room)
        {
            subRoom = new SubRoomInfo
            {
                Id = room.NextId++,
                Purpose = purpose,
                IsActive = true,
                CreatorId = creator.Id,
                Participants = [creator],
                MaximumParticipants = maximum,
                CreatedAt = DateTime.Now
            };
            room.SubRooms[subRoom.Id] = subRoom;
        }

        return subRoom;
    }

    bool ISubRoomManager.ActivateSubRoom(Guid roomId, int subRoomId, ParticipantInfo participant)
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

            if (subRoom.IsActive)
            {
                return false;
            }

            if (subRoom.Participants.Any(p => p.Id == participant.Id))
            {
                return false;
            }

            //subRoom.Participants.Add(participant);
            subRoom.IsActive = true;
            return true;
        }
    }

    SubRoomJoinOutcome ISubRoomManager.TryJoinSubRoom(Guid roomId, int subRoomId, ParticipantInfo participant)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return SubRoomJoinOutcome.RoomNotFound;
        }

        lock (room)
        {
            if (!room.SubRooms.TryGetValue(subRoomId, out var subRoom))
            {
                return SubRoomJoinOutcome.SubRoomNotFound;
            }

            if (subRoom.Participants.Any(p => p.Id == participant.Id))
            {
                return SubRoomJoinOutcome.AlreadyJoined;
            }

            if (subRoom.Participants.Count >= subRoom.MaximumParticipants)
            {
                return SubRoomJoinOutcome.MaximumParticipants;
            }

            subRoom.Participants.Add(participant);
            return SubRoomJoinOutcome.Success;
        }
    }

    bool ISubRoomManager.IsInSubRoom(Guid roomId, int subRoomId, Guid participantId)
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

            return subRoom.Participants.Any(p => p.Id == participantId);
        }
    }

    bool ISubRoomManager.LeaveSubRoom(Guid roomId, int subRoomId, Guid participantId)
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

            subRoom.Participants.RemoveAll(p => p.Id == participantId);

            if (subRoom.Participants.Count == 0)
            {
                subRoom.IsActive = false;
            }

            return subRoom.IsActive;
        }
    }

    bool ISubRoomManager.TryStopSubRoom(Guid roomId, int subRoomId, Guid requesterId)
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

            subRoom.Participants.Clear();
            subRoom.IsActive = false;
            return true;
        }
    }

    IReadOnlyList<Guid> ISubRoomManager.GetParticipantIds(Guid roomId, int subRoomId)
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

            return subRoom.Participants.Select(x => x.Id).ToList().AsReadOnly();
        }
    }

    int ISubRoomManager.GetParticipantCount(Guid roomId, int subRoomId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return 0;
        }

        lock (room)
        {
            if (!room.SubRooms.TryGetValue(subRoomId, out var subRoom))
            {
                return 0;
            }

            return subRoom.Participants.Count;
        }
    }

    SubRoomInfo? ISubRoomManager.GetSubRoom(Guid roomId, int subRoomId)
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

    IReadOnlyList<SubRoomInfo> ISubRoomManager.GetRoomSubRooms(Guid roomId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return [];
        }

        lock (room)
        {
            return room.SubRooms.Values
                .Select(sr => sr with { Participants = sr.Participants.ToList() })
                .ToList()
                .AsReadOnly();
        }
    }

    void ISubRoomManager.ClearRoomSubRooms(Guid roomId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
        {
            return;
        }

        lock (room)
        {
            room.SubRooms.Clear();
        }
    }
}
