namespace MIN.Services.Contracts.Models;

/// <summary>
/// Найденная комната
/// </summary>
public class DiscoveredRoom
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Название комнаты
    /// </summary>
    public string RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Максимальное количество участников
    /// </summary>
    public int MaximumParticipants { get; set; }

    /// <summary>
    /// Идентификатор хоста
    /// </summary>
    public Guid HostId { get; set; }

    /// <summary>
    /// Имя хоста
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// Имя компьютера хоста
    /// </summary>
    public string HostPCName { get; set; } = string.Empty;

    /// <summary>
    /// Текущее количество участников
    /// </summary>
    public int CurrentParticipants { get; set; }
}
