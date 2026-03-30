namespace MIN.Core.Entities.Contracts.Interfaces;

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
