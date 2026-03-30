using MIN.Core.Entities.Contracts.Interfaces;

namespace MIN.Core.Entities;

/// <summary>
/// Участник комнаты
/// </summary>
public class Participant : IParticipantData
{
    /// <summary>
    /// Идентификатор участника
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Имя участника
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
