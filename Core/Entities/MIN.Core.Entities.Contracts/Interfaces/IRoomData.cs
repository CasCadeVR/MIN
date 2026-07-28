using MIN.Core.Entities.Contracts.Models;

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
    ParticipantInfo HostParticipant { get; }

    /// <summary>
    /// MODIFIED: Кабинет, в котором создалась комната
    /// </summary>
    string Cabinet { get; }

    /// <summary>
    /// MODIFIED: Номер компьютера, в котором создалась комната
    /// </summary>
    int PcNumber { get; }

    /// <summary>
    /// Максимальное количество участников
    /// </summary>
    int MaximumParticipants { get; }

    /// <summary>
    /// Текущее количество участников
    /// </summary>
    int ParticipantCount { get; }

    /// <summary>
    /// Всего количество сообщений
    /// </summary>
    int TotalMessageCount { get; }

    /// <summary>
    /// Активна ли комната
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Дата создания комнаты
    /// </summary>
    DateTime CreatedAt { get; }
}
