using System.Collections.Concurrent;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;

namespace MIN.Core.Stores.Registries;

/// <inheritdoc cref="IParticipantConnectionRegistry"/>
public sealed class ParticipantConnectionRegistry : IParticipantConnectionRegistry
{
    private readonly ConcurrentDictionary<RoomConnectionKey, RoomParticipantInfoKey> participantByConnectionId = new();
    private readonly ConcurrentDictionary<RoomParticipantIdKey, RoomConnectionKey> connectionIdByParticipantId = new();

    void IParticipantConnectionRegistry.Register(Guid roomId, Guid connectionId, ParticipantInfo participant)
    {
        participantByConnectionId[new RoomConnectionKey(roomId, connectionId)]
            = new RoomParticipantInfoKey(roomId, participant);

        connectionIdByParticipantId[new RoomParticipantIdKey(roomId, participant.Id)]
            = new RoomConnectionKey(roomId, connectionId);
    }

    void IParticipantConnectionRegistry.RegisterLocalParticipant(Guid roomId, ParticipantInfo participant)
    {
        participantByConnectionId[new RoomConnectionKey(roomId, CoreRegistryConstants.LocalConnectionId)]
            = new RoomParticipantInfoKey(roomId, participant);

        connectionIdByParticipantId[new RoomParticipantIdKey(roomId, participant.Id)]
            = new RoomConnectionKey(roomId, CoreRegistryConstants.LocalConnectionId);
    }

    void IParticipantConnectionRegistry.Unregister(Guid roomId, Guid connectionId)
    {
        if (participantByConnectionId.TryRemove(new RoomConnectionKey(roomId, connectionId), out var roomParticipantInfoKey))
        {
            connectionIdByParticipantId.TryRemove(new RoomParticipantIdKey(roomId, roomParticipantInfoKey.ParticipantInfo.Id), out _);
        }
    }

    ParticipantInfo IParticipantConnectionRegistry.GetParticipant(Guid roomId, Guid connectionId)
        => participantByConnectionId.TryGetValue(new RoomConnectionKey(roomId, connectionId), out var p)
        ? p.ParticipantInfo : throw new KeyNotFoundException();

    bool IParticipantConnectionRegistry.TryGetParticipantFromConnectionId(Guid roomId, Guid connectionId, out ParticipantInfo participant)
    {
        var result = participantByConnectionId.TryGetValue(new RoomConnectionKey(roomId, connectionId), out var roomParticipantInfoKey);
        participant = roomParticipantInfoKey.ParticipantInfo;
        return result;
    }

    Guid IParticipantConnectionRegistry.GetParticipantIdFromConnectionId(Guid roomId, Guid connectionId)
        => participantByConnectionId.TryGetValue(new RoomConnectionKey(roomId, connectionId), out var p)
        ? p.ParticipantInfo.Id : throw new KeyNotFoundException();

    Guid IParticipantConnectionRegistry.GetConnectionIdFromParticipantId(Guid roomId, Guid participantId)
        => connectionIdByParticipantId.TryGetValue(new RoomParticipantIdKey(roomId, participantId), out var roomConnectionKey)
        ? roomConnectionKey.ConnectionId : throw new KeyNotFoundException();

    bool IParticipantConnectionRegistry.TryGetConnectionIdFromParticipantId(Guid roomId, Guid participantId, out Guid connectionId)
    {
        var result = connectionIdByParticipantId.TryGetValue(new RoomParticipantIdKey(roomId, participantId), out var roomConnectionKey);
        connectionId = roomConnectionKey.ConnectionId;
        return result;
    }
}
