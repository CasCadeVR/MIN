using System.Collections.Concurrent;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces;

namespace MIN.Core.Services;

/// <inheritdoc cref="IParticipantRegistry"/>
public sealed class ParticipantRegistry : IParticipantRegistry
{
    private readonly ConcurrentDictionary<Guid, ParticipantInfo> participantsByConnectionId = new();
    private readonly ConcurrentDictionary<Guid, Guid> connectionIdByParticipantId = new();

    void IParticipantRegistry.SetParticipantInfo(Guid connectionId, ParticipantInfo participant)
    {
        participantsByConnectionId[connectionId] = participant;
        connectionIdByParticipantId[participant.Id] = connectionId;
    }

    ParticipantInfo? IParticipantRegistry.GetParticipantInfo(Guid connectionId)
        => participantsByConnectionId.TryGetValue(connectionId, out var participant) ? participant : null;

    Guid IParticipantRegistry.GetConnectionIdFromParticipantId(Guid participantId)
    {
        if (connectionIdByParticipantId.TryGetValue(participantId, out var connectionId))
        {
            return connectionId;
        }

        throw new KeyNotFoundException($"ConnectionId not found for participant {participantId}");
    }

    bool IParticipantRegistry.TryGetParticipantInfo(Guid connectionId, out ParticipantInfo participant)
        => participantsByConnectionId.TryGetValue(connectionId, out participant);

    void IParticipantRegistry.RemoveParticipantInfo(Guid connectionId)
    {
        participantsByConnectionId.TryRemove(connectionId, out _);
    }
}
