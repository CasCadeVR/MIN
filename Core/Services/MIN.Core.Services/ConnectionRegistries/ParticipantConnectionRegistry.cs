using System.Collections.Concurrent;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Constants;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;

namespace MIN.Core.Services.ConnectionRegistries;

/// <inheritdoc cref="IParticipantConnectionRegistry"/>
public sealed class ParticipantConnectionRegistry : IParticipantConnectionRegistry
{
    private readonly ConcurrentDictionary<Guid, ParticipantInfo> participantsByConnectionId = new();
    private readonly ConcurrentDictionary<Guid, Guid> connectionIdByParticipantId = new();

    void IParticipantConnectionRegistry.Register(Guid connectionId, ParticipantInfo participant)
    {
        participantsByConnectionId[connectionId] = participant;
        connectionIdByParticipantId[participant.Id] = connectionId;
    }

    void IParticipantConnectionRegistry.RegisterLocalParticipant(ParticipantInfo participant)
    {
        participantsByConnectionId[CoreServicesConstants.LocalConnectionId] = participant;
        connectionIdByParticipantId[participant.Id] = CoreServicesConstants.LocalConnectionId;
    }

    ParticipantInfo IParticipantConnectionRegistry.GetParticipant(Guid connectionId)
        => participantsByConnectionId.TryGetValue(connectionId, out var p) ? p : throw new KeyNotFoundException();

    bool IParticipantConnectionRegistry.TryGetParticipantFromConnectionId(Guid connectionId, out ParticipantInfo participant)
        => participantsByConnectionId.TryGetValue(connectionId, out participant!);

    void IParticipantConnectionRegistry.Unregister(Guid connectionId)
    {
        if (participantsByConnectionId.TryRemove(connectionId, out var participant))
        {
            connectionIdByParticipantId.TryRemove(participant.Id, out _);
        }
    }

    Guid IParticipantConnectionRegistry.GetConnectionIdFromParticipantId(Guid participantId)
        => connectionIdByParticipantId.TryGetValue(participantId, out var connectionId) ? connectionId : throw new KeyNotFoundException();

    bool IParticipantConnectionRegistry.TryGetConnectionIdFromParticipantId(Guid participantId, out Guid connectionId)
        => connectionIdByParticipantId.TryGetValue(participantId, out connectionId);
}
