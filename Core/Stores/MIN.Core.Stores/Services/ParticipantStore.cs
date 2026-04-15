using System.Collections.Concurrent;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.Stores.Services;

/// <inheritdoc cref="IParticipantStore"/>
public sealed class ParticipantStore : IParticipantStore
{
    private readonly ConcurrentDictionary<Guid, List<ParticipantInfo>> participants = new();

    void IParticipantStore.AddParticipant(Guid roomId, ParticipantInfo participant)
    {
        var participants = this.participants.GetOrAdd(roomId, _ => []);
        lock (participants)
        {
            if (!participants.Any(p => p.Id == participant.Id))
            {
                participants.Add(participant);
            }
        }
    }

    void IParticipantStore.RemoveParticipant(Guid roomId, Guid participantId)
    {
        if (participants.TryGetValue(roomId, out var roomParticipants))
        {
            lock (roomParticipants)
            {
                var existing = roomParticipants.FirstOrDefault(p => p.Id == participantId);
                if (existing != null)
                {
                    roomParticipants.Remove(existing);
                }
            }
        }
    }

    ParticipantInfo IParticipantStore.GetParticipantById(Guid roomId, Guid participantId)
    {
        if (participants.TryGetValue(roomId, out var roomParticipants))
        {
            lock (roomParticipants)
            {
                return roomParticipants.FirstOrDefault(x => x.Id == participantId)
                    ?? throw new ArgumentNullException(nameof(participantId));
            }
        }
        else
        {
            throw new ArgumentNullException(nameof(roomId));
        }
    }

    bool IParticipantStore.TryGetParticipantById(Guid roomId, Guid participantId, out ParticipantInfo? participantInfo)
    {
        if (participants.TryGetValue(roomId, out var roomParticipants))
        {
            lock (roomParticipants)
            {
                participantInfo = roomParticipants.FirstOrDefault(x => x.Id == participantId);
                return participantInfo != null;
            }
        }
        else
        {
            throw new ArgumentNullException(nameof(roomId));
        }
    }

    IEnumerable<ParticipantInfo> IParticipantStore.GetParticipants(Guid roomId)
    {
        if (participants.TryGetValue(roomId, out var roomParticipants))
        {
            lock (roomParticipants)
            {
                return roomParticipants.ToList();
            }
        }

        return [];
    }

    void IParticipantStore.ClearParticipants(Guid roomId)
    {
        participants.TryRemove(roomId, out _);
    }
}
