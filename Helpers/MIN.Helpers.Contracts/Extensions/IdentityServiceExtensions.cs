using MIN.Core.Entities.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Helpers.Contracts.Extensions;

/// <summary>
/// Расширения для <see cref="IIdentityService"/>
/// </summary>
public static class IdentityServiceExtensions
{
    /// <summary>
    /// Преобразовать <see cref="IParticipantData"/> в <see cref="ParticipantInfo"/> 
    /// </summary>
    public static ParticipantInfo ToParticipantInfo(this IParticipantData participant)
        => new()
        {
            Id = participant.Id,
            Name = participant.Name,
        };
}
