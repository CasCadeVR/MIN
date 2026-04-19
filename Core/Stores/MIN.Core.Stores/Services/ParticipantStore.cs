using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.Stores.Services;

/// <inheritdoc cref="IParticipantStore"/>
public sealed class ParticipantStore : IParticipantStore
{
    private readonly List<ParticipantInfo> participants = [];

    void IParticipantStore.AddParticipant(ParticipantInfo participant)
    {
        lock (participants)
        {
            if (!participants.Any(p => p.Id == participant.Id))
            {
                participants.Add(participant);
            }
        }
    }

    void IParticipantStore.RemoveParticipant(Guid participantId)
    {
        lock (participants)
        {
            var existing = participants.FirstOrDefault(p => p.Id == participantId);
            if (existing != null)
            {
                participants.Remove(existing);
            }
        }
    }

    ParticipantInfo IParticipantStore.GetParticipantById(Guid participantId)
    {
        lock (participants)
        {
            return participants.FirstOrDefault(x => x.Id == participantId)
                ?? throw new ArgumentNullException(nameof(participantId));
        }
    }

    bool IParticipantStore.TryGetParticipantById(Guid participantId, out ParticipantInfo? participantInfo)
    {
        lock (participants)
        {
            participantInfo = participants.FirstOrDefault(x => x.Id == participantId);
            return participantInfo != null;
        }
    }

    IEnumerable<ParticipantInfo> IParticipantStore.GetParticipants()
    {
        lock (participants)
        {
            return participants.ToList();
        }
    }

    void IParticipantStore.ClearParticipants()
    {
        participants.Clear();
    }
}
