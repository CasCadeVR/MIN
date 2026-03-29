using MIN.Entities.Contracts.Models;

namespace MIN.Discovery.Services.Contracts.Interfaces;

/// <summary>
/// Сервис обнаружения комнат в сети
/// </summary>
public interface IDiscoveryService
{
    /// <summary>
    /// Запустить процесс обнаружения своей комнаты в сети
    /// </summary>
    Task StartDiscoveryAsync(RoomInfo room, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить процесс обнаружения своей комнаты в сети
    /// </summary>
    Task StopDiscoveryAsync();

    /// <summary>
    /// Обнаружить комнаты
    /// </summary>
    Task DiscoverRoomsAsync(string? searchZone, TimeSpan timeout, CancellationToken cancellationToken = default);
}
