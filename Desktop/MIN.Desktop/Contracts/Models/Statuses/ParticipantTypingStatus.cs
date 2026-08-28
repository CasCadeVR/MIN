using System;
using System.Collections.Generic;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Desktop.Contracts.Interfaces;

namespace MIN.Desktop.Contracts.Models.Statuses;

/// <summary>
/// UI Статус печатания участника
/// </summary>
public record struct ParticipantTypingStatus(Guid StatusId, List<string> ParticipantNames) : IDescribableStatus
{
    /// <inheritdoc />
    public readonly Guid Id => StatusId;

    /// <inheritdoc />
    readonly string IDescribable.GetDescription()
    {
        if (ParticipantNames.Count == 1)
        {
            return $"{ParticipantNames[0]} печатает...";
        }
        else if (ParticipantNames.Count == 2)
        {
            return $"{ParticipantNames[0]} и {ParticipantNames[1]} печатают...";
        }
        else if (ParticipantNames.Count == 3)
        {
            return $"{ParticipantNames[0]}, {ParticipantNames[1]} и {ParticipantNames[2]} печатают...";
        }
        else
        {
            return "Несколько участников печатают...";
        }
    }
}
