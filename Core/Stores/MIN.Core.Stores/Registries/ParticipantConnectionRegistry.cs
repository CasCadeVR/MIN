using System.Collections.Concurrent;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;

namespace MIN.Core.Stores.Registries;

/// <inheritdoc cref="IParticipantConnectionRegistry"/>
public sealed class ParticipantConnectionRegistry : IParticipantConnectionRegistry
{
    private readonly ConcurrentDictionary<Guid, ParticipantInfo> participantByConnectionId = new(); // <ConnectionId, ParticipantInfo>
    private readonly ConcurrentDictionary<Guid, Guid> connectionIdByParticipantId = new(); // <ParticipantId, ConnectionId>

    void IParticipantConnectionRegistry.Register(Guid connectionId, ParticipantInfo participant)
    {
        participantByConnectionId[connectionId] = participant;
        connectionIdByParticipantId[participant.Id] = connectionId;
    }

    void IParticipantConnectionRegistry.RegisterLocalParticipant(ParticipantInfo participant)
    {
        participantByConnectionId[CoreRegistryConstants.LocalConnectionId] = participant;
        connectionIdByParticipantId[participant.Id] = CoreRegistryConstants.LocalConnectionId;
    }

    void IParticipantConnectionRegistry.Unregister(Guid connectionId)
    {
        if (participantByConnectionId.TryRemove(connectionId, out var participant))
        {
            connectionIdByParticipantId.TryRemove(participant.Id, out _);
        }
    }

    ParticipantInfo IParticipantConnectionRegistry.GetParticipant(Guid connectionId)
        => participantByConnectionId.TryGetValue(connectionId, out var p) ? p : throw new KeyNotFoundException();

    bool IParticipantConnectionRegistry.TryGetParticipantFromConnectionId(Guid connectionId, out ParticipantInfo participant)
        => participantByConnectionId.TryGetValue(connectionId, out participant!);

    Guid IParticipantConnectionRegistry.GetParticipantIdFromConnectionId(Guid connectionId)
        => participantByConnectionId.TryGetValue(connectionId, out var p) ? p.Id : throw new KeyNotFoundException();

    Guid IParticipantConnectionRegistry.GetConnectionIdFromParticipantId(Guid participantId)
        => connectionIdByParticipantId.TryGetValue(participantId, out var connectionId) ? connectionId : throw new KeyNotFoundException();

    bool IParticipantConnectionRegistry.TryGetConnectionIdFromParticipantId(Guid participantId, out Guid connectionId)
        => connectionIdByParticipantId.TryGetValue(participantId, out connectionId);
}
