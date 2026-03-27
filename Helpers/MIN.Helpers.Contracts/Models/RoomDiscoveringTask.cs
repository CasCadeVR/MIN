using MIN.Entities.Contracts.Models;

namespace MIN.Helpers.Contracts.Models;

/// <summary>
/// Модель задачи поиска комнаты
/// </summary>
public class RoomDiscoveringTask
{
    /// <summary>
    /// Имя компьютера комнаты для поиска
    /// </summary>
    public string PcName { get; set; } = string.Empty;

    /// <summary>
    /// Задача поиска комнаты
    /// </summary>
    public Task<RoomInfo?> Task { get; set; } = null!;
}
