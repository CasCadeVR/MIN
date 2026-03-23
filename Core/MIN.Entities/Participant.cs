using MIN.Entities.Contracts;

namespace MIN.Entities;

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

    /// <summary>
    /// Имя компьютера участника
    /// </summary>
    public string PCName { get; set; } = string.Empty;
}
