using MIN.Core.Entities.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Entities.Contracts.Models;

/// <summary>
/// Данные участника для передачи по сети
/// </summary>
public record ParticipantInfo : IParticipantData
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Точка подключения к участнику
    /// </summary>
    public IEndpoint? Endpoint { get; set; }

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
    public ParticipantInfo(string name)
    {
        Name = name;
    }
}
