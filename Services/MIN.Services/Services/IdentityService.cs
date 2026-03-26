using MIN.Entities.Contracts;
using MIN.Services.Contracts.Interfaces;

namespace MIN.Services.Services
{
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
}
