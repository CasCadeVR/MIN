using System.ComponentModel;

namespace MIN.Voice.Services.Models;

/// <summary>
/// Запись канала звука с громкотию для одного участника
/// </summary>
internal sealed record ParticipantVoiceEntry(ParticipantChannel Channel, PropertyChangedEventHandler VolumeHandler);
