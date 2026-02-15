namespace MIN.Services.Contracts.Models;

/// <summary>
/// Найденная комната
/// </summary>
public class DiscoveredRoom
{
    /// <summary>
    /// Комната
    /// </summary>
    public Room Room { get; set; } = null!;

    /// <summary>
    /// Когда была найдена
    /// </summary>
    public DateTime DiscoveredAt { get; set; }
}
