using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IVoicePlaybackService"/>
public class VoicePlaybackService : IVoicePlaybackService
{
    private readonly List<Guid> paricipants = [];

    void IVoicePlaybackService.PlaySamples(Guid participantId, long sequenceNumber, byte[] data)
    {
        // TODO
    }

    void IVoicePlaybackService.AddParticipant(Guid participantId) => paricipants.Add(participantId);

    void IVoicePlaybackService.Clear() => paricipants.Clear();

    void IVoicePlaybackService.RemoveParticipant(Guid participantId) => paricipants.Remove(participantId);


    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {

    }
}
