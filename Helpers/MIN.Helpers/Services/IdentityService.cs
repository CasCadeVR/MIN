using MIN.Core.Entities.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Helpers.Services;

/// <inheritdoc />
public sealed class IdentityService : IIdentityService
{
    private IParticipantData currentParticipant = new ParticipantInfo()
    {
        Name = "Ты"
    };

    IParticipantData IIdentityService.SelfPartcipant => currentParticipant;

    /// <inheritdoc />
    void IIdentityService.SetParticipant(IParticipantData participantData)
    {
        currentParticipant = participantData;
    }

    /// <inheritdoc />
    void IIdentityService.ResetParticipant(IParticipantData participantData)
    {
        currentParticipant = null!;
    }
}
