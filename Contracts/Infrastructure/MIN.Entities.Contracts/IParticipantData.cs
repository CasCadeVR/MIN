namespace MIN.Entities.Contracts;

/// <summary>
/// Данные участника
/// </summary>
public interface IParticipantData
{
    /// <summary>
    /// Идентификатор участника
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Имя участника
    /// </summary>
    string Name { get; }
}
