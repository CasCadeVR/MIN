using System.Collections.Concurrent;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Core.Services.Stores
{
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
}
