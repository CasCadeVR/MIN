using MIN.Core.Entities.Contracts.Models;

namespace MIN.Voice.Services.Contacts.Models
{
    /// <summary>
    /// Состояние звонка в комнате
    /// </summary>
    public sealed record VoiceCallState(int? ActiveSubRoomId,
        DateTime? StartedAt,
        IReadOnlyList<ParticipantInfo> Participants);
}
