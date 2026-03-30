namespace MIN.Core.Entities.Contracts.Interfaces;

/// <summary>
/// Данные комнаты
/// </summary>
public interface IRoomData
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Имя комнаты
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Хост комнаты
    /// </summary>
    IParticipantData HostParticipant { get; }

    /// <summary>
    /// Максимальное количество участников
    /// </summary>
    int MaximumParticipants { get; }

    /// <summary>
    /// Текущее количество участников
    /// </summary>
    int ParticipantCount { get; }

    /// <summary>
    /// Активна ли комната
    /// </summary>
    bool IsActive { get; }
}
