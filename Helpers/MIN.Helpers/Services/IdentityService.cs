using MIN.Core.Entities.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Helpers.Services;

/// <inheritdoc />
public sealed class IdentityService : IIdentityService
{
    private IParticipantData currentParticipant = null!;

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
