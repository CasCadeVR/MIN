using MIN.Core.Entities.Contracts.Interfaces;

namespace MIN.Core.Entities.Contracts.Models;

/// <summary>
/// Данные участника для передачи по сети
/// </summary>
public record ParticipantInfo : IParticipantData
{
    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantInfo"/>
    /// </summary>
    /// <param name="participant"></param>
    public ParticipantInfo(IParticipantData participant)
    {
        Id = participant.Id;
        Name = participant.Name;
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantInfo"/>
    /// </summary>
    public ParticipantInfo(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
